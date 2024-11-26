using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TrainingGuides;
using TrainingGuides.Web.Features.Membership.Profile;

[assembly: RegisterPageTemplate(
    identifier: ProfilePagePageTemplate.IDENTIFIER,
    name: "Article page content type template",
    customViewName: "~/Features/Articles/ArticlePagePageTemplate.cshtml",
    ContentTypeNames = [ProfilePage.CONTENT_TYPE_NAME],
    IconClass = "xp-a-lowercase")]

namespace TrainingGuides.Web.Features.Membership.Profile;
public static class ProfilePagePageTemplate
{
    public const string IDENTIFIER = "TrainingGuides.ProfilePagePageTemplate";
}