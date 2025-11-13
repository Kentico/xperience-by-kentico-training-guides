using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.Scheduler;
using TrainingGuides.Web.Features.ContactImport;

[assembly: RegisterScheduledTask(identifier: ContactishRecipientTransferScheduledTask.IDENTIFIER, typeof(ContactishRecipientTransferScheduledTask))]

namespace TrainingGuides.Web.Features.ContactImport;

public class ContactishRecipientTransferScheduledTask(
    IInfoProvider<ContactGroupMemberInfo> contactGroupMemberInfoProvider,
    IContactImportService contactImportService) : IScheduledTask
{
    public const string IDENTIFIER = "TrainingGuides.ContactishRecipientTransferScheduledTask";

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

        // Delete these members from the target recipient list
        // Avoid this approach if you do not want the source contact group to be the source of truth for your recipient list.
        contactGroupMemberInfoProvider.BulkDelete(new WhereCondition()
            .WhereIn(nameof(ContactGroupMemberInfo.ContactGroupMemberRelatedID), topDeletableRecipients)
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberContactGroupID), targetRecipientList.ContactGroupID)
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberType), ContactGroupMemberTypeEnum.Contact));

        return await Task.FromResult(ScheduledTaskExecutionResult.Success);
    }
}