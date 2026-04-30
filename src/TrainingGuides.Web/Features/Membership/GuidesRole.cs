using CMS.Membership;
using Kentico.Membership;

namespace TrainingGuides.Web.Features.Membership;

public class GuidesRole : ApplicationRole
{
    public const string BADGE_LABEL_FIELD_NAME = "GuidesRoleBadgeLabel";
    public const string BADGE_COLOR_FIELD_NAME = "GuidesRoleBadgeColor";
    public const string BENEFITS_SUMMARY_FIELD_NAME = "GuidesRoleBenefitsSummary";

    public string BadgeLabel { get; set; } = string.Empty;

    public string BadgeColor { get; set; } = string.Empty;

    public string BenefitsSummary { get; set; } = string.Empty;

    public override void MapFromMemberRoleInfo(MemberRoleInfo source)
    {
        base.MapFromMemberRoleInfo(source);

        BadgeLabel = source.GetValue(BADGE_LABEL_FIELD_NAME, string.Empty);
        BadgeColor = source.GetValue(BADGE_COLOR_FIELD_NAME, string.Empty);
        BenefitsSummary = source.GetValue(BENEFITS_SUMMARY_FIELD_NAME, string.Empty);
    }

    public override void MapToMemberRoleInfo(MemberRoleInfo target)
    {
        base.MapToMemberRoleInfo(target);

        target.SetValue(BADGE_LABEL_FIELD_NAME, BadgeLabel);
        target.SetValue(BADGE_COLOR_FIELD_NAME, BadgeColor);
        target.SetValue(BENEFITS_SUMMARY_FIELD_NAME, BenefitsSummary);
    }
}
