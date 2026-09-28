using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.ContentPromotion.Services;

public class ContentPromotionService(
    IContentItemRetrieverService contentItemRetrieverService,
    IWebPageUrlRetriever webPageUrlRetriever)
    : IContentPromotionService
{
    private const int LINKED_ITEMS_DEPTH = 3;

    public async Task<PromotedItemResult> ResolvePromotedItem(ContentPromotionWidgetProperties properties)
    {
        if (properties.ContentSource == ContentPromotionSource.MANUAL)
        {
            return new PromotedItemResult();
        }

        bool fromContentHub = properties.ContentSource == ContentPromotionSource.CONTENT_ITEM;

        var selectedGuid = (fromContentHub ? properties.SelectedContentItem : properties.SelectedPage)
            .Select(selected => selected.Identifier)
            .FirstOrDefault();

        if (selectedGuid == Guid.Empty)
        {
            return new PromotedItemResult();
        }

        object? item = fromContentHub
            ? await contentItemRetrieverService.RetrieveContentItemByGuid(selectedGuid, LINKED_ITEMS_DEPTH)
            : await contentItemRetrieverService.RetrieveWebPageByContentItemGuid(selectedGuid, LINKED_ITEMS_DEPTH);

        if (item is null)
        {
            return new PromotedItemResult { SelectionFailed = true };
        }

        var result = item switch
        {
            ArticlePage articlePage => FromArticle(articlePage.ArticlePageArticleContent?.FirstOrDefault()),
            ProductPage productPage => FromProduct(productPage.ProductPageProducts?.FirstOrDefault()),
            ServicePage servicePage => FromService(servicePage.ServicePageService?.FirstOrDefault()),
            IArticleSchema article => FromArticle(article),
            Service service => FromService(service),
            IProductSchema product => FromProduct(product),
            _ => new PromotedItemResult()
        };

        // Retrieval succeeded but nothing usable came back - an unsupported content type, or a
        // page whose linked content item is missing. The editor did select something, so this
        // is a broken selection rather than an empty one.
        result.SelectionFailed = result.Item is null;

        // Kept so the link rules can reuse the page that was already retrieved, instead of
        // querying for the same page a second time.
        result.Page = item as IWebPageFieldsSource;

        return result;
    }

    /// <summary>
    /// Turns the widget's image override into a view model. Kept out of the resolution rules
    /// because it needs content retrieval, and kept out of the view component so that "an empty
    /// asset means no image" is decided in one place.
    /// </summary>
    public async Task<AssetViewModel?> ResolveOverrideImage(ContentPromotionWidgetProperties properties)
    {
        var imageGuid = properties.Image.Select(image => image.Identifier).FirstOrDefault();

        return imageGuid == Guid.Empty
            ? null
            : GetImage(await contentItemRetrieverService.RetrieveContentItemByGuid<Asset>(imageGuid));
    }

    /// <summary>
    /// <see cref="AssetViewModel.GetViewModel"/> returns an empty model rather than null for a
    /// missing asset, which would read as "there is an image" everywhere downstream - the card
    /// would then render an <c>img</c> with no source.
    /// </summary>
    private static AssetViewModel? GetImage(Asset? asset)
    {
        var image = AssetViewModel.GetViewModel(asset);

        return string.IsNullOrWhiteSpace(image.FilePath) ? null : image;
    }

    private static PromotedItemResult FromArticle(IArticleSchema? article) => article is null
        ? new PromotedItemResult()
        : new PromotedItemResult
        {
            Family = ContentFamily.Article,
            Item = new PromotedItemSource
            {
                Title = article.ArticleSchemaTitle,
                Description = article.ArticleSchemaSummary,
                Image = GetImage(article.ArticleSchemaTeaser?.FirstOrDefault())
            }
        };

    private static PromotedItemResult FromProduct(IProductSchema? product) => product is null
        ? new PromotedItemResult()
        : new PromotedItemResult
        {
            Family = ContentFamily.Product,
            Item = new PromotedItemSource
            {
                Title = product.ProductSchemaName,
                Description = product.ProductSchemaDescription
            }
        };

    public async Task<LinkViewModel?> ResolveLink(
        ContentPromotionWidgetProperties properties,
        IWebPageFieldsSource? selectedPage)
    {
        var destinationPage = properties.ContentSource == ContentPromotionSource.PAGE
            ? selectedPage
            : await RetrieveLinkTargetPage(properties);

        // A link target page wins over a typed URL. The form hides the URL input once a page
        // is selected, but hiding a field does not clear its stored value, so both can still
        // arrive here.
        string url = destinationPage is not null
            ? (await webPageUrlRetriever.Retrieve(destinationPage)).RelativePath
            : properties.LinkUrl;

        return string.IsNullOrWhiteSpace(url)
            ? null
            : new LinkViewModel
            {
                LinkUrl = url,
                OpenInNewTab = properties.OpenInNewTab
            };
    }

    private async Task<IWebPageFieldsSource?> RetrieveLinkTargetPage(ContentPromotionWidgetProperties properties)
    {
        var targetGuid = properties.LinkTargetPage.Select(page => page.Identifier).FirstOrDefault();

        return targetGuid == Guid.Empty
            ? null
            : await contentItemRetrieverService.RetrieveWebPageByContentItemGuid(targetGuid, LINKED_ITEMS_DEPTH);
    }

    public ContentPromotionDisplayValues ResolveDisplayValues(
        ContentPromotionWidgetProperties properties,
        PromotedItemSource? item,
        AssetViewModel? overrideImage = null) => new()
        {
            Title = Resolve(properties, ContentPromotionElement.TITLE, properties.Title, item?.Title),
            Description = Resolve(properties, ContentPromotionElement.DESCRIPTION, properties.Description, item?.Description),
            CallToActionText = Resolve(properties, ContentPromotionElement.CALL_TO_ACTION, properties.CallToActionText, item?.CallToActionText),
            Image = ResolveImage(properties, overrideImage, item?.Image)
        };

    private static PromotedItemResult FromService(Service? service) => service is null
        ? new PromotedItemResult()
        : new PromotedItemResult
        {
            Family = ContentFamily.Service,
            Item = new PromotedItemSource
            {
                Title = service.ServiceName,
                Description = service.ServiceShortDescription,
                Image = GetImage(service.ServiceMedia?.FirstOrDefault())
            }
        };

    /// <summary>
    /// The image override arrives already resolved, because turning the selected asset
    /// reference into an <see cref="AssetViewModel"/> needs content retrieval, which does
    /// not belong in this rule.
    /// </summary>
    private static AssetViewModel? ResolveImage(
        ContentPromotionWidgetProperties properties,
        AssetViewModel? overrideImage,
        AssetViewModel? inheritedImage)
    {
        if (properties.HideElements.Contains(ContentPromotionElement.IMAGE))
        {
            return null;
        }

        return overrideImage ?? inheritedImage;
    }

    /// <summary>
    /// Hide beats override, override beats inherit.
    /// </summary>
    private static string Resolve(
        ContentPromotionWidgetProperties properties,
        string element,
        string overrideValue,
        string? inheritedValue)
    {
        if (properties.HideElements.Contains(element))
        {
            return string.Empty;
        }

        return string.IsNullOrWhiteSpace(overrideValue)
            ? inheritedValue ?? string.Empty
            : overrideValue;
    }
}
