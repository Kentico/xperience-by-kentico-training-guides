using System.Xml;
using CMS.ContactManagement;

namespace TrainingGuides.Web.Features.ContactImport;

public interface IContactImportService
{
    /// <summary>
    /// Imports contacts from the provided XML document.
    /// </summary>
    /// <param name="document">Xml document containing contacts</param>
    /// <returns>The number of contacts inserted</returns>
    int ImportContactsFromXml(XmlDocument document);

    /// <summary>
    /// Ensures that the contact group(s) for imported contacts exists in the database.
    /// </summary>
    /// <param name="rebuildContactGroup">Indicates whether to rebuild the contact group after ensuring its existence</param>
    Task EnsureContactGroup(bool rebuildContactGroup);

    /// <summary>
    /// Ensures that the recipient list(s) for imported contacts exists in the database (both <see cref="ContactGroupInfo"/> and <see cref="CMS.EmailMarketing.RecipientListSettingsInfo"/>).
    /// </summary>
    Task EnsureRecipientLists();

    /// <summary>
    /// Gets the Contact Group ID from cache or database if not found in cache.
    /// </summary>
    /// <param name="contactGroupCodeName">Code name of the contact group to fetch</param>
    /// <returns>The contact group if found, otherwise null</returns>
    Task<ContactGroupInfo?> GetContactGroupCached(string contactGroupCodeName);

    /// <summary>
    /// Gets a contact group by its code name from the database.
    /// </summary>
    /// <param name="contactGroupCodeName">Code name of the contact group to fetch</param>
    /// <returns>The contact group if found, otherwise null</returns>
    Task<ContactGroupInfo?> GetContactGroup(string contactGroupCodeName);

    /// <summary>
    /// Creates or updates a contact group member binding for a recipient list, corresponding to the provided standard contact group member.
    /// </summary>
    /// <param name="contactGroupMember">Member of the contact group to copy to the recipient list</param>
    /// <param name="recipientListContactGroupId">ID of the recipient list contact group</param>
    Task CreateOrUpdateRecipient(int contactId, int recipientListContactGroupId);

    /// <summary>
    /// Checks whether the contact is already in the specified recipient list.
    /// </summary>
    /// <param name="contactId">ID of contact to check</param>
    /// <param name="recipientListContactGroupId">ID of the recipient list contact group</param>
    /// <returns>True if a contact group member exists for the specified contact and recipient list</returns>
    Task<bool> ContactAlreadyInGroup(int contactId, int recipientListContactGroupId);

    /// <summary>
    /// Upserts a recipient in the recipient list based on the provided contact group member.
    /// </summary>
    /// <param name="contactId">ID of the contact to upsert as a recipient</param>
    Task UpsertRecipient(int contactId);

    /// <summary>
    /// Gets members one contact group that are not in another contact group.
    /// </summary>
    /// <param name="contactGroupX">The contact group to get members from</param>
    /// <param name="contactGroupY">The contact group to compare against</param>
    /// <param name="topN">The maximum number of members to return</param>
    /// <returns>A collection of contact IDs</returns>
    Task<IEnumerable<int>> GetGroupXMembersNotInGroupY(ContactGroupInfo contactGroupX,
        ContactGroupInfo contactGroupY,
        int topN);

    /// <summary>
    /// Logs a warning that the specified contact group was not found.
    /// </summary>
    /// <param name="contactGroupName">The name of the contact group that was not found</param>
    void LogMissingContactGroup(string contactGroupName);

    /// <summary>
    /// Deletes the specified contacts from the specified recipient list, including their subscription confirmations and contact group member bindings.
    /// </summary>
    /// <param name="contactIds">IDs of the contacts to remove</param>
    /// <param name="recipientListContactGroupId">ID of the recipient list contact group</param>
    Task DeleteRecipients(IEnumerable<int> contactIds, int recipientListContactGroupId);
}