using CMS.Membership;
using Kentico.Membership;

namespace TrainingGuides.Web.Features.Membership;

public class GuidesMember : ApplicationUser
{
    public string GivenName { get; set; } = "";
    public string FamilyName { get; set; } = "";
    public bool FamilyNameFirst { get; set; } = false;
    public string FullName =>
        (GivenName, FamilyName) switch
        {
            ("", "") => "",
            (string given, "") => given,
            ("", string family) => family,
            (string given, string family) =>
                FamilyNameFirst ? $"{family} {given}" : $"{given} {family}",
            (null, null) or _ => "",
        };

    public string FavoriteCoffee { get; set; } = "";
    public DateTime Created { get; set; }



    public override void MapToMemberInfo(MemberInfo target)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        /*
         * base.MapToMemberInfo will set target.MemberPassword everytime
         * however we do not want to set it if PasswordHash is null,
         * and this stores the original so we can revert it
         */
        string originalPasswordHash = target.MemberPassword;

        base.MapToMemberInfo(target);

        if (PasswordHash is null)
        {
            target.MemberPassword = originalPasswordHash;
        }

        _ = target.SetValue("MemberGivenName", GivenName);
        _ = target.SetValue("MemberFamilyName", FamilyName);
        _ = target.SetValue("FamilyNameFirst", FamilyNameFirst);
        _ = target.SetValue("MemberFavoriteCoffee", FavoriteCoffee);
    }

    public override void MapFromMemberInfo(MemberInfo source)
    {
        base.MapFromMemberInfo(source);

        GivenName = source.GetValue("MemberGivenName", "");
        FamilyName = source.GetValue("MemberFamilyName", "");
        FamilyNameFirst = source.GetValue("FamilyNameFirst", false);
        FavoriteCoffee = source.GetValue("MemberFavoriteCoffee", "");
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
    public static GuidesMember AsMember(this MemberInfo member) =>
        GuidesMember.FromMemberInfo(member);
}