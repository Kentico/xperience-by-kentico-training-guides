using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TrainingGuides;
using TrainingGuides.Web.Features.Articles;

[assembly: RegisterPageTemplate(
    identifier: ArticlePagePageTemplate.IDENTIFIER,
    name: "Article page content type template",
    customViewName: "~/Features/Articles/ArticlePagePageTemplate.cshtml",
    ContentTypeNames = [ArticlePage.CONTENT_TYPE_NAME],
    IconClass = "xp-a-lowercase")]

namespace TrainingGuides.Web.Features.Articles;

public static class ArticlePagePageTemplate
{
    public const string IDENTIFIER = "TrainingGuides.ArticlePagePageTemplate";
}