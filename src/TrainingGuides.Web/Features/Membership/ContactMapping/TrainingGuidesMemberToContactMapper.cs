using CMS;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.Membership;

using Kentico.OnlineMarketing.Web.Mvc;

using TrainingGuides.Web.Features.Membership.ContactMapping;

[assembly: RegisterImplementation(typeof(IMemberToContactMapper), typeof(TrainingGuidesMemberToContactMapper))]

namespace TrainingGuides.Web.Features.Membership.ContactMapping;
public class TrainingGuidesMemberToContactMapper : IMemberToContactMapper
{
    private readonly IInfoProvider<ContactInfo> contactInfoProvider;

    public TrainingGuidesMemberToContactMapper(IInfoProvider<ContactInfo> contactInfoProvider)
    {
        this.contactInfoProvider = contactInfoProvider;
    }

    /// <summary>
    /// Maps a member to a contact and updates the contact if it has changed
    /// </summary>
    /// <param name="member">The member whose data should be transferred</param>
    /// <param name="contact">The contact to transfer the data to</param>
    public void Map(MemberInfo member, ContactInfo contact)
    {
        if (member is null || contact is null)
            return;

        contact = TransferMemberFieldsToContact(member, contact);

        UpdateContactIfChanged(contact);
    }

    /// <summary>
    /// Transfers values from member to contact
    /// </summary>
    /// <param name="member">The member whose data should be transferred</param>
    /// <param name="contact">The contact to transfer the data to</param>
    /// <returns>The updated ContactInfo object, but DOES NOT save the contact data</returns>
    public ContactInfo TransferMemberFieldsToContact(MemberInfo member, ContactInfo contact)
    {
        var guidesMember = member.AsGuidesMember();

        if (!string.IsNullOrWhiteSpace(guidesMember.GivenName))
        {
            contact.ContactFirstName = guidesMember.GivenName;
        }
        if (!string.IsNullOrWhiteSpace(guidesMember.FamilyName))
        {
            _ = contact.ContactLastName = guidesMember.FamilyName;
        }
        if (!string.IsNullOrWhiteSpace(guidesMember.FavoriteCoffee))
        {
            _ = contact.SetValue("TrainingGuidesContactFavoriteCoffee", guidesMember.FavoriteCoffee);
        }

        // Sets the Member ID of the current contact
        contact.SetValue("TrainingGuidesContactMemberId", guidesMember.Id);

        // For data security, do not overwrite contact email address if it is already set
        if (string.IsNullOrWhiteSpace(contact.ContactEmail) && !string.IsNullOrWhiteSpace(guidesMember.Email))
        {
            contact.ContactEmail = guidesMember.Email;
        }

        return contact;
    }

    private void UpdateContactIfChanged(ContactInfo contact)
    {
        if (contact.HasChanged)
        {
            contactInfoProvider.Set(contact);
        }
    }
}