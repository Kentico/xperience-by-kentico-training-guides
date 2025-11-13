using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.EmailMarketing;
using CMS.Scheduler;
using TrainingGuides.Web.Features.ContactImport;

[assembly: RegisterScheduledTask(identifier: ContactishContactDeleteScheduledTask.IDENTIFIER, typeof(ContactishContactDeleteScheduledTask))]

namespace TrainingGuides.Web.Features.ContactImport;

public class ContactishContactDeleteScheduledTask(IInfoProvider<ContactInfo> contactInfoProvider,
    IInfoProvider<ContactGroupInfo> contactGroupInfoProvider,
    IInfoProvider<ContactGroupMemberInfo> contactGroupMemberInfoProvider,
    IInfoProvider<EmailSubscriptionConfirmationInfo> emailSubscriptionConfirmationInfoProvider,
    IInfoProvider<RecipientListSettingsInfo> recipientListSettingsInfoProvider) : IScheduledTask
{
    public const string IDENTIFIER = "TrainingGuides.ContactishContactDeleteScheduledTask";

    // This example shows how to clear out test data while developing contact import functionality.
    // If you use something like this in your own development, remember to delete it once you start working with live data.
    public async Task<ScheduledTaskExecutionResult> Execute(ScheduledTaskConfigurationInfo task, CancellationToken cancellationToken)
    {
        // Where condition to select contacts imported from Contactish
        var contactWhere = new WhereCondition()
            .WhereNotNull(ContactishValues.IDENTIFIER_FIELD);

        // Where condition to select contact groups related to Contactish imports
        var contactGroupWhere = new WhereCondition()
            .WhereIn(nameof(ContactGroupInfo.ContactGroupName),
            [
                ContactishValues.ContactGroupName,
                ContactishValues.RecipientListName
            ]);

        // Get contact IDs to delete related data
        var contactIds = await contactInfoProvider.Get()
            .Where(contactWhere)
            .Column(nameof(ContactInfo.ContactID))
            .GetListResultAsync<int>();

        // Get contact group IDs to delete related data
        var contactGroupIds = await contactGroupInfoProvider.Get()
            .Where(contactGroupWhere)
            .Column(nameof(ContactGroupInfo.ContactGroupID))
            .GetListResultAsync<int>();

        // Delete subscriptions for the contacts we will remove
        emailSubscriptionConfirmationInfoProvider.BulkDelete(new WhereCondition()
            .WhereIn(nameof(EmailSubscriptionConfirmationInfo.EmailSubscriptionConfirmationContactID), contactIds));

        // Delete contact group members for the groups we will remove
        contactGroupMemberInfoProvider.BulkDelete(new WhereCondition()
            .WhereIn(nameof(ContactGroupMemberInfo.ContactGroupMemberContactGroupID), contactGroupIds)
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberType), ContactGroupMemberTypeEnum.Contact));

        // Delete recipient list settings for the groups we will remove
        recipientListSettingsInfoProvider.BulkDelete(new WhereCondition()
            .WhereIn(nameof(RecipientListSettingsInfo.RecipientListSettingsRecipientListID), contactGroupIds));

        // Delete the contacts
        contactInfoProvider.BulkDelete(contactWhere);

        // Delete the contact groups
        // BulkDelete ignores recipient lists so we need to delete these one by one
        contactGroupInfoProvider.Get()
            .Where(contactGroupWhere)
            .ForEachObject(contactGroup => contactGroup.Delete());

        return await Task.FromResult(ScheduledTaskExecutionResult.Success);
    }
}