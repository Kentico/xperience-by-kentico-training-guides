using CMS.Membership;
using Kentico.Membership;

namespace TrainingGuides.Web.Features.Membership;

public class GuidesMember : ApplicationUser
{
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public bool FamilyNameFirst { get; set; } = false;
    public string FullName =>
        (GivenName, FamilyName) switch
        {
            ("", "") => string.Empty,
            (string given, "") => given,
            ("", string family) => family,
            (string given, string family) =>
                FamilyNameFirst ? $"{family} {given}" : $"{given} {family}",
            (null, null) or _ => string.Empty,
        };

    public string FavoriteCoffee { get; set; } = string.Empty;
    public DateTime Created { get; set; }

    public override void MapToMemberInfo(MemberInfo target)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        /*
         * base.MapToMemberInfo will set target.MemberPassword every time
         * however we do not want to set it if PasswordHash is null,
         * and this stores the original so we can revert it
         */
        string originalPasswordHash = target.MemberPassword;

        base.MapToMemberInfo(target);

        if (PasswordHash is null)
        {
            target.MemberPassword = originalPasswordHash;
        }

        _ = target.SetValue("GuidesMemberGivenName", GivenName);
        _ = target.SetValue("GuidesMemberFamilyName", FamilyName);
        _ = target.SetValue("GuidesMemberFamilyNameFirst", FamilyNameFirst);
        _ = target.SetValue("GuidesMemberFavoriteCoffee", FavoriteCoffee);
    }

    public override void MapFromMemberInfo(MemberInfo source)
    {
        base.MapFromMemberInfo(source);

        GivenName = source.GetValue("GuidesMemberGivenName", string.Empty);
        FamilyName = source.GetValue("GuidesMemberFamilyName", string.Empty);
        FamilyNameFirst = source.GetValue("GuidesMemberFamilyNameFirst", false);
        FavoriteCoffee = source.GetValue("GuidesMemberFavoriteCoffee", string.Empty);
        Created = source.MemberCreated;
    }

    public static GuidesMember FromMemberInfo(MemberInfo memberInfo)
    {
        var guidesMember = new GuidesMember();
        guidesMember.MapFromMemberInfo(memberInfo);

        return guidesMember;
    }
}

public static class MemberInfoExtensions
{
    public static GuidesMember AsGuidesMember(this MemberInfo member) =>
        GuidesMember.FromMemberInfo(member);
}