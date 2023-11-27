using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TrainingGuides.Web.Features.Articles;
using TrainingGuides;

[assembly: RegisterPageTemplate(
    identifier: ArticlePagePageTemplate.IDENTIFIER,
    name: "Article page content type template",
    customViewName: "~/Features/Articles/ArticlePagePageTemplate.cshtml",
    ContentTypeNames = [ArticlePage.CONTENT_TYPE_NAME],
    IconClass = "xp-a-lowercase")]

namespace TrainingGuides.Web.Features.Articles;
public static class ArticlePagePageTemplate
{
    public const string IDENTIFIER = "TrainingGuides.ArticlePage";
}