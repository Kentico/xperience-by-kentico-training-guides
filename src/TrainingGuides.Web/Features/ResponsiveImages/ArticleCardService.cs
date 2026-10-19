using Kentico.Content.Web.Mvc;

namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// An article card: the article title and its teaser image.
/// </summary>
/// <param name="Title">The article title; empty when the page has no article.</param>
/// <param name="Image">The teaser image, or <c>null</c> when the article has none.</param>
public record ArticleCardViewModel(string Title, ImageViewModel? Image);

/// <summary>
/// Retrieves article pages as article cards.
/// </summary>
public class ArticleCardService(IContentRetriever contentRetriever)
{
    // The page links the article and the article links the teaser Asset,
    // so the asset is two levels deep.
    private const int TEASER_ASSET_DEPTH = 2;

    /// <summary>
    /// Gets a card for each article page in the current website channel.
    /// </summary>
    /// <returns>One card per article page, cached for the default expiration time.</returns>
    public Task<IEnumerable<ArticleCardViewModel>> GetArticleCards() =>
        contentRetriever.RetrievePages<ArticlePage, ArticleCardViewModel>(
            new RetrievePagesParameters { LinkedItemsMaxLevel = TEASER_ASSET_DEPTH },
            additionalQueryConfiguration: null,
            new RetrievalCacheSettings(cacheItemNameSuffix: nameof(ArticleCardViewModel)),
            // Receives each page already mapped to ArticlePage, linked items included
            (container, articlePage) =>
            {
                var article = articlePage.ArticlePageArticleContent.FirstOrDefault();

                return Task.FromResult(new ArticleCardViewModel(
                    article?.ArticleSchemaTitle ?? string.Empty,
                    ImageViewModel.GetViewModel(article?.ArticleSchemaTeaser.FirstOrDefault())));
            });
}
