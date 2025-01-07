using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.Membership;
using Kentico.Web.Mvc;
using TrainingGuides.Web.Features.DataProtection.Shared;

namespace TrainingGuides.Web.Features.Membership.Services;

public class MemberContactService : IMemberContactService
{
    private readonly IInfoProvider<ContactInfo> contactInfoProvider;
    private readonly ICookieAccessor cookieAccessor;
    private readonly ICurrentContactProvider currentContactProvider;
    private readonly IContactMergeService contactMergeService;

    private const string FAVORITE_COFFEE_FIELD_NAME = "TrainingGuidesContactFavoriteCoffee";
    private const string MEMBER_ID_FIELD_NAME = "TrainingGuidesContactMemberId";

    public MemberContactService(IInfoProvider<ContactInfo> contactInfoProvider,
        ICookieAccessor cookieAccessor,
        ICurrentContactProvider currentContactProvider,
        IContactMergeService contactMergeService)
    {
        this.contactInfoProvider = contactInfoProvider;
        this.cookieAccessor = cookieAccessor;
        this.currentContactProvider = currentContactProvider;
        this.contactMergeService = contactMergeService;
    }

    /// <inheritdoc />
    public ContactInfo TransferMemberFieldsToContact(MemberInfo member, ContactInfo contact)
    {
        var guidesMember = member.AsGuidesMember();

        return TransferMemberFieldsToContact(guidesMember, contact);
    }

    /// <inheritdoc />
    public ContactInfo TransferMemberFieldsToContact(GuidesMember guidesMember, ContactInfo contact)
    {
        var newContact = contact.Clone();

        if (!string.IsNullOrWhiteSpace(guidesMember.GivenName))
        {
            newContact.ContactFirstName = guidesMember.GivenName;
        }
        if (!string.IsNullOrWhiteSpace(guidesMember.FamilyName))
        {
            newContact.ContactLastName = guidesMember.FamilyName;
        }
        if (!string.IsNullOrWhiteSpace(guidesMember.FavoriteCoffee))
        {
            _ = newContact.SetValue(FAVORITE_COFFEE_FIELD_NAME, guidesMember.FavoriteCoffee);
        }

        // Sets the Member ID of the current contact
        _ = newContact.SetValue(MEMBER_ID_FIELD_NAME, guidesMember.Id);

        // For data security, do not overwrite contact email address if it is already set
        if (string.IsNullOrWhiteSpace(contact.ContactEmail) && !string.IsNullOrWhiteSpace(guidesMember.Email))
        {
            newContact.ContactEmail = guidesMember.Email;
        }

        return newContact;
    }

    /// <inheritdoc />
    public void UpdateContactIfChanged(ContactInfo contact)
    {
        if (contact.HasChanged)
        {
            contactInfoProvider.Set(contact);
        }
    }

    /// <summary>
    /// Gets the oldest contact associated with the provided member whose email matches
    /// </summary>
    /// <param name="member">The GuidesMember to find an associated contact</param>
    /// <returns>The oldest contact associated with the provided member whose email matches</returns>
    private ContactInfo? GetOldestMemberContactWithMatchingEmail(GuidesMember member)
    {
        var contact = contactInfoProvider.Get()
            .WhereEquals(MEMBER_ID_FIELD_NAME, member.Id)
            .WhereEquals(nameof(ContactInfo.ContactEmail), member.Email)
            .OrderBy(nameof(ContactInfo.ContactCreated))
            .TopN(1)
            .FirstOrDefault();

        return contact;
    }

    /// <inheritdoc />
    public void SetCurrentContactForMember(GuidesMember member)
    {
        var contact = GetOldestMemberContactWithMatchingEmail(member);
        if (contact is not null)
        {
            EnsureContactCookieLevel();
            currentContactProvider.SetCurrentContact(contact);
        }
    }

    /// <inheritdoc />
    public void MergeContactByEmail(ContactInfo contact) => contactMergeService.MergeContactByEmail(contact);

    /// <inheritdoc />
    public void RemoveContactCookies()
    {
        cookieAccessor.Remove(CookieNames.CURRENT_CONTACT);
        cookieAccessor.Remove(CookieNames.CMS_COOKIE_LEVEL);
        cookieAccessor.Remove(CookieNames.COOKIE_ACCEPTANCE);
        cookieAccessor.Remove(CookieNames.COOKIE_CONSENT_LEVEL);
    }

    /// <summary>
    /// Ensures that the CurrentContact cookie can be created by setting the CMS cookie level to 200
    /// </summary>
    /// <remarks>
    /// NOTE: In this project, the <see cref="DataProtection.ViewComponents.TrackingConsent.TrackingConsentViewComponent"/> will return the cookie level to 0 if the contact has not agreed to any consents. This level is necessary for to check for consent agreements and adjust cookie levels accordingly.
    /// </remarks>
    private void EnsureContactCookieLevel()
    {
        string cmsCookieLevel = cookieAccessor.Get(CookieNames.CMS_COOKIE_LEVEL);
        if (string.IsNullOrWhiteSpace(cmsCookieLevel) || !int.TryParse(cmsCookieLevel, out int cookieLevel) || cookieLevel < 200)
        {
            cookieAccessor.Set(CookieNames.CMS_COOKIE_LEVEL, "200");
        }
    }
}
