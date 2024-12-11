using CMS.ContactManagement;
using CMS.Membership;

namespace TrainingGuides.Web.Features.Membership.Services;

public interface IMemberContactService
{
    /// <summary>
    /// Transfers values from MemberInfo to ContactInfo
    /// </summary>
    /// <param name="member">The member whose data should be transferred</param>
    /// <param name="contact">The contact to transfer the data to</param>
    /// <returns>The updated ContactInfo object, but DOES NOT save the contact data</returns>
    ContactInfo TransferMemberFieldsToContact(MemberInfo member, ContactInfo contact);

    /// <summary>
    /// Transfers values from GuidesMember to ContactInfo
    /// </summary>
    /// <param name="guidesMember">The member whose data should be transferred</param>
    /// <param name="contact">The contact to transfer the data to</param>
    /// <returns>The updated ContactInfo object, but DOES NOT save the contact data</returns>
    ContactInfo TransferMemberFieldsToContact(GuidesMember guidesMember, ContactInfo contact);

    /// <summary>
    /// Saves the contact data if it has changed
    /// </summary>
    /// <param name="contact">The contact to save</param>
    void UpdateContactIfChanged(ContactInfo contact);

    /// <summary>
    /// Gets the oldest contact associated with the provided member whose email matches
    /// </summary>
    /// <param name="member">The GuidesMember to find an associated contact</param>
    /// <returns>The oldest contact associated with the provided member whose email matches</returns>
    ContactInfo? GetOldestMemberContactWithMatchingEmail(GuidesMember member);

    /// <summary>
    /// Sets the CurrentContact to the oldest one with a matching email that is associated with the given member 
    /// </summary>
    /// <param name="member">The member to find an associated contact for</param>
    void SetCurrentContactForMember(GuidesMember member);

    /// <summary>
    /// Merges the provided contact based on the provided email address
    /// </summary>
    /// <param name="contact">The contact to merge</param>
    public void MergeContactByEmail(ContactInfo contact);

    /// <summary>
    /// Removes contact related cookies 
    /// </summary>
    void RemoveContactCookies();
}