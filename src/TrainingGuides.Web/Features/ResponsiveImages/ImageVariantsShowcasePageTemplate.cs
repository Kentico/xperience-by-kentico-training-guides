using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using TrainingGuides;
using TrainingGuides.Web.Features.ResponsiveImages;

[assembly: RegisterPageTemplate(
    identifier: ImageVariantsShowcasePageTemplate.IDENTIFIER,
    name: "Image variants showcase",
    customViewName: "~/Features/ResponsiveImages/ImageVariantsShowcasePageTemplate.cshtml",
    ContentTypeNames = [ArticlePage.CONTENT_TYPE_NAME],
    IconClass = "xp-picture")]

namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// Article page template that renders the article's teaser image with image variants.
/// </summary>
public static class ImageVariantsShowcasePageTemplate
{
    /// <summary>
    /// Identifier of the page template.
    /// </summary>
    public const string IDENTIFIER = "TrainingGuides.ImageVariantsShowcasePageTemplate";
}
