using CMS.Scheduler;
using TrainingGuides.Web.Features.ContactImport;

[assembly: RegisterScheduledTask(identifier: ContactishRecipientTransferScheduledTask.IDENTIFIER, typeof(ContactishRecipientTransferScheduledTask))]

namespace TrainingGuides.Web.Features.ContactImport;

public class ContactishRecipientTransferScheduledTask(
    IContactImportService contactImportService) : IScheduledTask
{
    public const string IDENTIFIER = "TrainingGuides.ContactishRecipientTransferScheduledTask";

    // Keep in mind that each contact we transfer will lead to two database queries in the upsert method - one to check if it already exists, and a second to insert the recipient.
    // Consider being conservative with batch size if you plan to run this task during peak hours, or if you have a large number of contacts.
    public const int BatchSize = 25;

    // This scheduled task moves batches of contacts from the source contact group to the target recipient list, and deletes any contacts from the recipient list that are no longer in the source contact group
    public async Task<ScheduledTaskExecutionResult> Execute(ScheduledTaskConfigurationInfo task, CancellationToken cancellationToken)
    {
        var sourceContactGroup = await contactImportService.GetContactGroupCached(ContactishValues.ContactGroupName);
        var targetRecipientList = await contactImportService.GetContactGroupCached(ContactishValues.RecipientListName);

        if (sourceContactGroup is null)
        {
            contactImportService.LogMissingContactGroup(ContactishValues.ContactGroupName);
            return await Task.FromResult(new ScheduledTaskExecutionResult("Failed - Source contact group not found."));
        }
        if (targetRecipientList is null)
        {
            contactImportService.LogMissingContactGroup(ContactishValues.RecipientListName);
            return await Task.FromResult(new ScheduledTaskExecutionResult("Failed - Recipient list contact group not found."));
        }

        // Get members of source contact group not in target recipient list
        var topUnsyncedRecipients = await contactImportService.GetGroupXMembersNotInGroupY(sourceContactGroup, targetRecipientList, BatchSize);

        // Add these members as recipients in the target recipient list
        foreach (int contactId in topUnsyncedRecipients)
        {
            await contactImportService.UpsertRecipient(contactId);
        }

        // Get members of target recipient list not in source contact group
        // Since the contact group is how recipients are added to the list, this must mean the missing contacts were deleted
        var topDeletableRecipients = await contactImportService.GetGroupXMembersNotInGroupY(targetRecipientList, sourceContactGroup, BatchSize);

        if (topDeletableRecipients.Any())
        {
            // Delete these members from the target recipient list
            // Avoid this approach if you do not want the source contact group to be the source of truth for your recipient list.
            await contactImportService.DeleteRecipients(topDeletableRecipients, targetRecipientList.ContactGroupID);
        }

        return await Task.FromResult(ScheduledTaskExecutionResult.Success);
    }
}