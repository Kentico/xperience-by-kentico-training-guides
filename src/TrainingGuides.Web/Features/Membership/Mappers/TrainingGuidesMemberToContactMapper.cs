using CMS;
using CMS.ContactManagement;
using CMS.Membership;

using Kentico.OnlineMarketing.Web.Mvc;

using TrainingGuides.Web.Features.Membership.Mappers;

[assembly: RegisterImplementation(typeof(IMemberToContactMapper), typeof(TrainingGuidesMemberToContactMapper))]

namespace TrainingGuides.Web.Features.Membership.Mappers;
public class TrainingGuidesMemberToContactMapper : IMemberToContactMapper
{
    // Stores the default implementation of the IMemberToContactMapper service
    private readonly IMemberToContactMapper memberToContactMapper;

    public TrainingGuidesMemberToContactMapper(IMemberToContactMapper memberToContactMapper)
    {
        this.memberToContactMapper = memberToContactMapper;
    }

    public void Map(MemberInfo member, ContactInfo contact)
    {
        if (member is null || contact is null)
        {
            return;
        }
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

        //Sets the Member ID of the current contact
        contact.SetValue("TrainingGuidesContactMemberId", guidesMember.Id);

        memberToContactMapper.Map(member, contact);
    }
}