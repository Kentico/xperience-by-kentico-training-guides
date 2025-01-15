using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TrainingGuides;
using TrainingGuides.Web.Features.Membership.Profile;

[assembly: RegisterPageTemplate(
    identifier: ProfilePagePageTemplate.IDENTIFIER,
    name: "Profile page content type template",
    customViewName: "~/Features/Membership/Profile/ProfilePage/ProfilePagePageTemplate.cshtml",
    ContentTypeNames = [ProfilePage.CONTENT_TYPE_NAME],
    IconClass = "xp-personalisation")]

namespace TrainingGuides.Web.Features.Membership.Profile;
public static class ProfilePagePageTemplate
{
    public const string IDENTIFIER = "TrainingGuides.ProfilePagePageTemplate";
}