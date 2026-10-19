using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// Shows the teaser image of the current article page, rendered with the framework's image tag helpers
/// and with the responsive image tag helper.
/// </summary>
/// <param name="contentItemRetrieverService">Retrieves the current page.</param>
public class ImageVariantsShowcaseViewComponent(IContentItemRetrieverService contentItemRetrieverService) : ViewComponent
{
    // The page links the article and the article links the teaser Asset, so the asset is two levels deep
    private const int TEASER_ASSET_DEPTH = 2;

    /// <summary>
    /// Retrieves the current article page and maps its article's title and teaser image.
    /// </summary>
    /// <returns>The showcase view.</returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var articlePage = await contentItemRetrieverService.RetrieveCurrentPage<ArticlePage>(TEASER_ASSET_DEPTH);
        var article = articlePage?.ArticlePageArticleContent.FirstOrDefault();

        var model = new ImageVariantsShowcaseViewModel
        {
            Title = article?.ArticleSchemaTitle ?? string.Empty,
            Image = ImageViewModel.GetViewModel(article?.ArticleSchemaTeaser.FirstOrDefault())
        };

        return View("~/Features/ResponsiveImages/ImageVariantsShowcase.cshtml", model);
    }
}
