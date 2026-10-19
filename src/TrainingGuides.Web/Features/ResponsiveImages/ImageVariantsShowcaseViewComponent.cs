using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// Renders the current article page's teaser image using both the built-in Xperience
/// image tag helpers and the custom responsive image tag helper.
/// </summary>
/// <param name="contentItemRetrieverService">Service used to retrieve the current page.</param>
public class ImageVariantsShowcaseViewComponent(IContentItemRetrieverService contentItemRetrieverService) : ViewComponent
{
    // Level 0: ArticlePage -> Level 1: article item -> Level 2: teaser Asset item.
    private const int TEASER_ASSET_DEPTH = 2;

    /// <summary>
    /// Retrieves the current article page and maps its title and teaser image.
    /// </summary>
    /// <returns>The showcase view result.</returns>
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
