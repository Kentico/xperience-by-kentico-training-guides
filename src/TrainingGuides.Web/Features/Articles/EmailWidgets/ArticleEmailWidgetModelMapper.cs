using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Articles.EmailWidgets;

public class ArticleEmailWidgetModelMapper : IComponentModelMapper<ArticleEmailWidgetModel>
{
    private readonly IContentItemRetrieverService<ArticlePage> articleRetrieverService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ArticleEmailWidgetModel defaultModel = new()
    {
        ArticleTitle = "No article selected",
        ArticleTeaserImage = new ImageWidgetModel(),
        ArticleSummary = "Please select an article.",
        ArticleUrl = string.Empty,
    };

    public ArticleEmailWidgetModelMapper(
        IContentItemRetrieverService<ArticlePage> articleRetrieverService,
        IHttpContextAccessor httpContextAccessor)
    {
        this.articleRetrieverService = articleRetrieverService;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<ArticleEmailWidgetModel> Map(Guid contentItemGuid, string languageName)
    {
        if (contentItemGuid == Guid.Empty)
        {
            return defaultModel;
        }

        var articlePage = await articleRetrieverService.RetrieveWebPageByContentItemGuid(contentItemGuid, ArticlePage.CONTENT_TYPE_NAME, 2, languageName);

        string articlePageUrl = articlePage?.SystemFields.WebPageUrlPath is not null
            ? new UriBuilder()
            {
                Scheme = httpContextAccessor.HttpContext?.Request.Scheme,
                Host = httpContextAccessor.HttpContext?.Request.Host.Host,
                Port = httpContextAccessor.HttpContext?.Request.Host.Port ?? 80,
                Path = articlePage.SystemFields.WebPageUrlPath
            }.ToString()
            : string.Empty;

        var schemaArticle = articlePage?.ArticlePageArticleContent.FirstOrDefault();
        var oldArticle = articlePage?.ArticlePageContent.FirstOrDefault();

        var imageAsset = schemaArticle?.ArticleSchemaTeaser?.FirstOrDefault()
                ?? oldArticle?.ArticleTeaser?.FirstOrDefault();
        var imageUrl = new UriBuilder()
        {
            Scheme = httpContextAccessor.HttpContext?.Request.Scheme,
            Host = httpContextAccessor.HttpContext?.Request.Host.Host,
            Port = httpContextAccessor.HttpContext?.Request.Host.Port ?? 80,
            Path = imageAsset?.AssetFile?.Url.TrimStart('~')
        };

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
                    ImageUrl = imageUrl.ToString() ?? string.Empty,
                    AltText = imageAsset?.AssetAltText ?? string.Empty,
                },
                ArticleUrl = articlePageUrl
            }
            : defaultModel;
    }
}