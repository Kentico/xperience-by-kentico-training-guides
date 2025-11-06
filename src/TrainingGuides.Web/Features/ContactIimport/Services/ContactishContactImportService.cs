using System.Xml;
using CMS.Base;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataEngine.Query;
using CMS.EmailMarketing;
using CMS.Helpers;
using TrainingGuides.Web.Features.Shared.Logging;

namespace TrainingGuides.Web.Features.ContactImport;

public class ContactishContactImportService(IInfoProvider<ContactInfo> contactInfoProvider,
    IInfoProvider<ContactGroupInfo> contactGroupInfoProvider,
    IInfoProvider<RecipientListSettingsInfo> recipientListSettingsInfoProvider,
    IInfoProvider<ContactGroupMemberInfo> contactGroupMemberInfoProvider,
    IInfoProvider<EmailSubscriptionConfirmationInfo> emailSubscriptionConfirmationInfoProvider,
    IProgressiveCache progressiveCache,
    ICacheDependencyBuilderFactory cacheDependencyBuilderFactory,
    ILogger<ContactishContactImportService> logger) : IContactImportService
{
    /// <inheritdoc/>
    public int ImportContactsFromXml(XmlDocument document)
    {
        var contactInfos = GetContactsFromXml(document);

        // Note that object event handlers, contact group recalculation, etc. will not run for individual contacts added with BulkInsert
        // We need to manually invoke recalculation later in the process
        contactInfoProvider.BulkInsert(contactInfos);

        return contactInfos.Count();
    }

    public IEnumerable<ContactInfo> GetContactsFromXml(XmlDocument document)
    {
        List<ContactInfo> contactInfos = [];

        var contacts = document[ContactishValues.CONTACTS_ELEMENT];

        contacts?.ChildNodes
            .OfType<XmlElement>()
            .Where(x => x.Name == ContactishValues.CONTACT_ELEMENT)
            .ToList()
            .ForEach(contactElement =>
            {
                var contact = new ContactInfo()
                {
                    ContactFirstName = contactElement[ContactishValues.FIRST_NAME_ELEMENT]?.InnerText ?? string.Empty,
                    ContactLastName = contactElement[ContactishValues.LAST_NAME_ELEMENT]?.InnerText ?? string.Empty,
                    ContactEmail = contactElement[ContactishValues.EMAIL_ELEMENT]?.InnerText ?? string.Empty,
                    ContactCreated = DateTime.Now,
                    ContactGUID = Guid.NewGuid(),
                    ContactLastModified = DateTime.Now,
                };

                var LastUpdated = DateTime.TryParse(contactElement[ContactishValues.LAST_UPDATED_ELEMENT]?.InnerText, out var parsedDate)
                    ? parsedDate
                    : DateTime.MinValue;

                contact.SetValue(ContactishValues.IDENTIFIER_FIELD, contactElement[ContactishValues.IDENTIFIER_ELEMENT]?.InnerText ?? string.Empty);
                contact.SetValue(ContactishValues.SEGMENT_IDENTIFIERS_FIELD, contactElement[ContactishValues.SEGMENT_IDENTIFIERS_ELEMENT]?.InnerText ?? string.Empty);
                contact.SetValue(ContactishValues.LAST_UPDATED_FIELD, LastUpdated);
                contact.SetValue(ContactishValues.LAST_SYNCED_FIELD, DateTime.Now);

                if (!string.IsNullOrWhiteSpace(contact.ContactEmail) && !string.IsNullOrWhiteSpace(contact.GetValue(ContactishValues.IDENTIFIER_FIELD)?.ToString()))
                    contactInfos.Add(contact);
            });

        return contactInfos;
    }

    /// <summary>
    /// <inheritdoc/>
    /// Populates data based on <see cref="ContactishValues"/>
    /// </summary>
    public async Task EnsureContactGroup(bool rebuildContactGroup)
    {
        ContactGroupInfo contactGroup;

        var existingGroup = await GetContactGroup(ContactishValues.ContactGroupName);

        if (existingGroup is null)
        {
            var newGroup = new ContactGroupInfo()
            {
                ContactGroupName = ContactishValues.ContactGroupName,
                ContactGroupDisplayName = ContactishValues.ContactGroupDisplayName,
                ContactGroupDescription = ContactishValues.ContactGroupDescription,
                ContactGroupDynamicCondition = ContactishValues.ContactGroupDynamicCondition,
                ContactGroupEnabled = true,
                ContactGroupIsRecipientList = false
            };
            newGroup.Insert();
            contactGroup = newGroup;
        }
        else
        {
            existingGroup.ContactGroupName = ContactishValues.ContactGroupName;
            existingGroup.ContactGroupDescription = ContactishValues.ContactGroupDescription;
            existingGroup.ContactGroupDynamicCondition = ContactishValues.ContactGroupDynamicCondition;
            existingGroup.ContactGroupEnabled = true;
            existingGroup.ContactGroupIsRecipientList = false;

            existingGroup.Update();
            contactGroup = existingGroup;
        }

        if (rebuildContactGroup)
        {
            await RebuildContactGroup(contactGroup);
        }
    }

    // Note: This is an expensive operation, use it sparingly.
    private async Task RebuildContactGroup(ContactGroupInfo contactGroup)
    {
        contactGroup.ContactGroupStatus = ContactGroupStatusEnum.Rebuilding;
        contactGroup.Generalized.SetObject();

        // Invoke in new thread
        await Task.Factory.StartNew(CMSThread.Wrap(() =>
            {
                try
                {
                    if (contactGroup.ContactGroupStatus != ContactGroupStatusEnum.Rebuilding)
                    {
                        // Set status that the contact group is being rebuilt
                        contactGroup.ContactGroupStatus = ContactGroupStatusEnum.Rebuilding;
                        contactGroup.Update();
                    }

                    new ContactGroupRebuilder().RebuildGroup(contactGroup);
                }
                catch (Exception ex)
                {
                    logger.LogError(EventIds.ContactGroupRebuildFailed, ex, "Failed to rebuild contact group {ContactGroupName}.", contactGroup.ContactGroupName);
                    throw;
                }
                finally
                {
                    // Return to ready status
                    contactGroup.ContactGroupStatus = ContactGroupStatusEnum.Ready;
                    contactGroup.Update();
                }
            }),
            TaskCreationOptions.LongRunning);
    }

    /// <summary>
    /// <inheritdoc/>
    /// Populates data based on <see cref="ContactishValues"/>
    /// </summary>
    public async Task EnsureRecipientLists()
    {
        var existingGroup = await GetContactGroup(ContactishValues.RecipientListName);

        ContactGroupInfo contactGroup;

        if (existingGroup is null)
        {
            var newGroup = new ContactGroupInfo()
            {
                ContactGroupName = ContactishValues.RecipientListName,
                ContactGroupDisplayName = ContactishValues.RecipientListDisplayName,
                ContactGroupDescription = ContactishValues.RecipientListDescription,
                ContactGroupDynamicCondition = string.Empty,
                ContactGroupIsRecipientList = true
            };

            newGroup.Insert();

            contactGroup = newGroup;
        }
        else
        {
            existingGroup.ContactGroupName = ContactishValues.RecipientListName;
            existingGroup.ContactGroupDescription = ContactishValues.RecipientListDescription;
            existingGroup.ContactGroupDynamicCondition = string.Empty;

            existingGroup.Update();

            contactGroup = existingGroup;
        }

        await EnsureRecipientListSettings(contactGroup);
    }

    /// <inheritdoc/>
    public async Task EnsureRecipientListSettings(ContactGroupInfo? recipientList)
    {
        if (recipientList is null)
        {
            return;
        }

        var existingListSettings = (await recipientListSettingsInfoProvider.Get()
            .WhereEquals(nameof(RecipientListSettingsInfo.RecipientListSettingsRecipientListID), recipientList.ContactGroupID)
            .GetEnumerableTypedResultAsync())
            .FirstOrDefault();

        if (existingListSettings is null)
        {
            var settings = new RecipientListSettingsInfo
            {
                RecipientListSettingsRecipientListID = recipientList.ContactGroupID,

                RecipientListSettingsAfterConfirmationPage = ContactishValues.RecipientListThankYouPageGuid,
                RecipientListSettingsSendSubscriptionConfirmationEmail = false,
                RecipientListSettingsSubscriptionConfirmationEmailID = default,

                RecipientListSettingsAfterUnsubscriptionPage = ContactishValues.RecipientListGoodbyePageGuid,
                RecipientListSettingsSendUnsubscriptionConfirmationEmail = false,
                RecipientListSettingsUnsubscriptionConfirmationEmailID = default,

            };

            settings.Insert();
        }
        else
        {
            existingListSettings.RecipientListSettingsAfterConfirmationPage = ContactishValues.RecipientListThankYouPageGuid;
            existingListSettings.RecipientListSettingsSendSubscriptionConfirmationEmail = false;
            existingListSettings.RecipientListSettingsSubscriptionConfirmationEmailID = default;

            existingListSettings.RecipientListSettingsAfterUnsubscriptionPage = ContactishValues.RecipientListGoodbyePageGuid;
            existingListSettings.RecipientListSettingsSendUnsubscriptionConfirmationEmail = false;
            existingListSettings.RecipientListSettingsUnsubscriptionConfirmationEmailID = default;

            existingListSettings.Update();
        }

    }

    /// <inheritdoc/>
    public async Task<ContactGroupInfo?> GetContactGroupCached(string contactGroupCodeName) =>
        await progressiveCache.LoadAsync(
            async cacheSettings =>
            {
                cacheSettings.Cached = true;
                var contactGroup = await GetContactGroup(contactGroupCodeName);

                if (contactGroup is not null)
                {
                    cacheSettings.CacheDependency = cacheDependencyBuilderFactory.Create()
                        .ForInfoObjects<ContactGroupInfo>()
                            // .ById(contactGroup.ContactGroupID)
                            // .ByCodeName(contactGroupCodeName)
                            // .ByGuid(contactGroup.ContactGroupGUID)
                            .All()
                            .Builder()
                        .Build();
                }

                return contactGroup;
            },
            new CacheSettings(cacheMinutes: 60,
                useSlidingExpiration: true,
                cacheItemNameParts: [nameof(ContactishContactImportService),
                    nameof(GetContactGroupCached),
                    contactGroupCodeName]));

    /// <inheritdoc/>
    public async Task<ContactGroupInfo?> GetContactGroup(string contactGroupCodeName)
    {
        var contactGroup = await contactGroupInfoProvider.GetAsync(contactGroupCodeName);

        return contactGroup;
    }

    /// <inheritdoc/>
    public async Task CreateOrUpdateRecipient(int contactId, int recipientListContactGroupId)
    {

        var existingRecipient = (await contactGroupMemberInfoProvider.Get()
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberRelatedID), contactId)
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberType), ContactGroupMemberTypeEnum.Contact)
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberContactGroupID), recipientListContactGroupId)
            .GetEnumerableTypedResultAsync()).FirstOrDefault();

        if (existingRecipient is null)
        {
            var recipient = new ContactGroupMemberInfo()
            {
                // Use the same contact for contact group binding
                ContactGroupMemberRelatedID = contactId,
                ContactGroupMemberType = ContactGroupMemberTypeEnum.Contact,

                // Bind the object to the recipient list contact group
                ContactGroupMemberContactGroupID = recipientListContactGroupId,

                // Indicate that the contact was added manually (not via dynamic condition)
                ContactGroupMemberFromManual = true,

            };

            recipient.Insert();

            await EnsureSubscriptionConfirmation(contactId, recipientListContactGroupId);
        }
        else
        {
            // Update the existing recipient, only touch the values we don't already know from the where condition of the query.
            existingRecipient.ContactGroupMemberFromManual = true;
            existingRecipient.ContactGroupMemberFromCondition = false;
            existingRecipient.ContactGroupMemberFromAccount = false;

            if (existingRecipient.HasChanged)
            {
                existingRecipient.Update();
            }
        }

    }

    public async Task EnsureSubscriptionConfirmation(int contactId, int recipientListContactGroupId)
    {
        var existingConfirmation = (await emailSubscriptionConfirmationInfoProvider.Get()
            .WhereEquals(nameof(EmailSubscriptionConfirmationInfo.EmailSubscriptionConfirmationContactID), contactId)
            .WhereEquals(nameof(EmailSubscriptionConfirmationInfo.EmailSubscriptionConfirmationRecipientListID), recipientListContactGroupId)
            .GetEnumerableTypedResultAsync())
            .FirstOrDefault();

        if (existingConfirmation is null)
        {
            var confirmation = new EmailSubscriptionConfirmationInfo()
            {
                EmailSubscriptionConfirmationContactID = contactId,
                EmailSubscriptionConfirmationRecipientListID = recipientListContactGroupId,
                // Assume they are not unsubscribed if we are adding them as a recipient
                EmailSubscriptionConfirmationIsApproved = true,
                // Optionally query the contact or external system for a more accurate date
                EmailSubscriptionConfirmationDate = DateTime.Now,
            };

            confirmation.Insert();
        }
    }

    public async Task<bool> ContactAlreadyInGroup(int contactId, int recipientListContactGroupId)
    {
        int recipientExists = await contactGroupMemberInfoProvider.Get()
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberRelatedID), contactId)
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberType), ContactGroupMemberTypeEnum.Contact)
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberContactGroupID), recipientListContactGroupId)
            .GetCountAsync();

        return recipientExists > 0;
    }

    // public void UpsertRecipient(int contactId)
    // {
    //     int? recipientListContactGroupId = GetContactGroupCached(ContactishContactGroupConstants.RecipientListName).Result?.ContactGroupID;


    //     if (recipientListContactGroupId is null or 0)
    //     {
    //         LogMissingContactGroup(ContactishContactGroupConstants.RecipientListName);
    //         return;
    //     }

    //     CreateOrUpdateRecipient(contactId, (int)recipientListContactGroupId).Wait();
    // }

    /// <inheritdoc/>
    public async Task UpsertRecipient(int contactId)
    {
        int? recipientListContactGroupId = (await GetContactGroupCached(ContactishValues.RecipientListName))?.ContactGroupID;

        if (recipientListContactGroupId is null or 0)
        {
            LogMissingContactGroup(ContactishValues.RecipientListName);
            return;
        }

        await CreateOrUpdateRecipient(contactId, (int)recipientListContactGroupId);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<int>> GetGroupXMembersNotInGroupY(ContactGroupInfo contactGroupX,
        ContactGroupInfo contactGroupY,
        int topN)
    {
        var GroupYIds = contactGroupMemberInfoProvider.Get()
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberContactGroupID), contactGroupY.ContactGroupID)
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberType), ContactGroupMemberTypeEnum.Contact)
            .Column(nameof(ContactGroupMemberInfo.ContactGroupMemberRelatedID));

        var groupXMembersNotInY = await contactGroupMemberInfoProvider.Get()
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberContactGroupID), contactGroupX.ContactGroupID)
            .WhereNotIn(nameof(ContactGroupMemberInfo.ContactGroupMemberRelatedID), GroupYIds)
            .WhereEquals(nameof(ContactGroupMemberInfo.ContactGroupMemberType), ContactGroupMemberTypeEnum.Contact)
            .TopN(topN)
            .Column(nameof(ContactGroupMemberInfo.ContactGroupMemberRelatedID))
            .GetListResultAsync<int>();

        return groupXMembersNotInY;
    }

    public void LogMissingContactGroup(string contactGroupName) => logger.LogError(EventIds.ContactGroupNotFound, "Recipient list contact group {ContactGroupName} not found for Contactish recipients.", contactGroupName);

}