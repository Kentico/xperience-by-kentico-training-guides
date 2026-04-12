namespace TrainingGuides.Web.Features.Membership;

public static class GuidesRoleNames
{
    public const string BASIC_MEMBER = "BasicMember";
    public const string ENTHUSIAST_MEMBER = "EnthusiastMember";
    public const string INSIDER_MEMBER = "InsiderMember";

    public static readonly IReadOnlyList<string> DisplayPriority =
    [
        INSIDER_MEMBER,
        ENTHUSIAST_MEMBER,
        BASIC_MEMBER
    ];
}
