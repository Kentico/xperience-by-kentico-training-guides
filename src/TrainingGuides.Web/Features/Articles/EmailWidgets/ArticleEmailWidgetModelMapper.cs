using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Articles.EmailWidgets;

public class ArticleEmailWidgetModelMapper : IComponentModelMapper<ArticleEmailWidgetModel>
{
    private readonly IContentItemRetrieverService<ArticlePage> articleRetrieverService;
    private readonly IStringLocalizer<SharedResources> stringLocalizer;

    private ArticleEmailWidgetModel DefaultModel => new()
    {
        ArticleTitle = stringLocalizer["No article selected"],
        ArticleTeaserImage = new ImageWidgetModel(),
        ArticleSummary = stringLocalizer["Please select an article."],
        ArticleUrl = string.Empty,
    };


    public ArticleEmailWidgetModelMapper(
        IContentItemRetrieverService<ArticlePage> articleRetrieverService,
        IStringLocalizer<SharedResources> stringLocalizer)
    {
        this.articleRetrieverService = articleRetrieverService;
        this.stringLocalizer = stringLocalizer;
    }

    public async Task<ArticleEmailWidgetModel> Map(Guid contentItemGuid, string languageName)
    {
        if (contentItemGuid == Guid.Empty)
        {
            return DefaultModel;
        }

        var articlePage = await articleRetrieverService.RetrieveWebPageByContentItemGuid(contentItemGuid, ArticlePage.CONTENT_TYPE_NAME, 2, languageName);

        string articlePageUrl = articlePage.GetUrl().AbsoluteUrl;

        var schemaArticle = articlePage?.ArticlePageArticleContent.FirstOrDefault();
        var oldArticle = articlePage?.ArticlePageContent.FirstOrDefault();

        var imageAsset = schemaArticle?.ArticleSchemaTeaser?.FirstOrDefault()
                ?? oldArticle?.ArticleTeaser?.FirstOrDefault();
        string imageUrl = imageAsset?.AssetFile?.Url ?? string.Empty;

        return (articlePage is not null && (schemaArticle is not null || oldArticle is not null))
            ? new ArticleEmailWidgetModel()
            {
                ArticleTitle = schemaArticle?.ArticleSchemaTitle
                    ?? oldArticle?.ArticleTitle
                    ?? string.Empty,
                ArticleSummary = schemaArticle?.ArticleSchemaSummary
                    ?? oldArticle?.ArticleSummary
                    ?? string.Empty,
                ArticleTeaserImage = new ImageWidgetModel
                {
                    ImageUrl = imageUrl.ToString(),
                    AltText = imageAsset?.AssetAltText ?? string.Empty,
                },
                ArticleUrl = articlePageUrl
            }
            : DefaultModel;
    }
}