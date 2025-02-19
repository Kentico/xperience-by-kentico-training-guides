using CMS;
using CMS.ContactManagement;
using CMS.Membership;

using Kentico.OnlineMarketing.Web.Mvc;

using TrainingGuides.Web.Features.Membership.Services;

[assembly: RegisterImplementation(typeof(IMemberToContactMapper), typeof(TrainingGuidesMemberToContactMapper))]

namespace TrainingGuides.Web.Features.Membership.Services;
public class TrainingGuidesMemberToContactMapper : IMemberToContactMapper
{
    private readonly IMemberContactService memberContactService;

    public TrainingGuidesMemberToContactMapper(IMemberContactService memberContactService)
    {
        this.memberContactService = memberContactService;
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

        contact = memberContactService.TransferMemberFieldsToContact(member, contact);

        memberContactService.UpdateContactIfChanged(contact);
    }
}