using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using CMS.ContentEngine;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Commerce.Products.Models;
using TrainingGuides.Web.Commerce.Products.Services;
using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.ContentPromotion.Services;

public partial class ContentPromotionService(
    IContentItemRetrieverService contentItemRetrieverService,
    IWebPageUrlRetriever webPageUrlRetriever,
    ITaxonomyRetriever taxonomyRetriever,
    IPreferredLanguageRetriever preferredLanguageRetriever,
    IProductService productService)
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

        // Both calls name the content types the matching selector offered. That is what lets the
        // query return content type-specific fields at all - see IContentItemRetrieverService.
        object? item = fromContentHub
            ? await contentItemRetrieverService.RetrieveContentItemByGuid(
                selectedGuid, ContentPromotionContentTypes.CONTENT_ITEMS, LINKED_ITEMS_DEPTH)
            : await contentItemRetrieverService.RetrieveWebPageByContentItemGuid(
                selectedGuid, ContentPromotionContentTypes.PAGES, LINKED_ITEMS_DEPTH);

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
        // A hidden image element throws the result away, so do not pay for the query at all.
        if (properties.HideElements.Contains(ContentPromotionElement.IMAGE))
        {
            return null;
        }

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
            PromotedContent = article,
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
            PromotedContent = product,
            Item = new PromotedItemSource
            {
                Title = product.ProductSchemaName,
                Description = product.ProductSchemaDescription,
                Image = GetProductImage(product)
            }
        };

    /// <summary>
    /// The product families keep their images in <c>ProductImage</c> content items rather than
    /// in assets, so they need their own mapping - <see cref="AssetViewModel.GetViewModel"/>
    /// takes an <see cref="Asset"/>, and nothing in this chain is one.
    /// </summary>
    /// <remarks>
    /// A product page links a parent product, and a parent may carry no images of its own while
    /// its variants do. The product page and the listing both fall back to a variant image for
    /// that reason; the card needs only one image, so it takes the first one it can find.
    /// </remarks>
    private static AssetViewModel? GetProductImage(IProductSchema product) =>
        ImagesOf(product)
            .Concat(VariantsOf(product).SelectMany(ImagesOf))
            .Select(GetImage)
            .FirstOrDefault(image => image is not null);

    private static IEnumerable<ProductImage> ImagesOf(IProductSchema product) =>
        product.ProductSchemaImages ?? [];

    private static IEnumerable<IProductSchema> VariantsOf(IProductSchema product) =>
        (product as IProductParentSchema)?.ProductParentSchemaVariants?.OfType<IProductSchema>() ?? [];

    /// <summary>
    /// Mirrors <see cref="GetImage(Asset?)"/>: a product image with no file behind it is no
    /// image, not an image with an empty source.
    /// </summary>
    private static AssetViewModel? GetImage(ProductImage? productImage)
    {
        string? url = productImage?.ProductImageAsset?.Url;

        return string.IsNullOrWhiteSpace(url)
            ? null
            : new AssetViewModel
            {
                FilePath = url,
                AltText = productImage!.ProductImageAltText ?? string.Empty
            };
    }

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

    public async Task<PromotionExtrasViewModel> ResolveExtras(
        ContentPromotionWidgetProperties properties,
        PromotedItemResult promotedItem)
    {
        if (!properties.ShowExtras)
        {
            return new PromotionExtrasViewModel();
        }

        return promotedItem.PromotedContent switch
        {
            IArticleSchema article => await ArticleExtras(article),
            Service service => ServiceExtras(service),
            IProductSchema product => await ProductExtras(product),
            _ => new PromotionExtrasViewModel()
        };
    }

    private async Task<PromotionExtrasViewModel> ArticleExtras(IArticleSchema article)
    {
        var categoryGuids = (article.ArticleSchemaCategory ?? [])
            .Select(category => category.Identifier)
            .ToList();

        if (categoryGuids.Count == 0)
        {
            return new PromotionExtrasViewModel();
        }

        var tags = await taxonomyRetriever.RetrieveTags(categoryGuids, preferredLanguageRetriever.Get());

        return new PromotionExtrasViewModel
        {
            Categories = tags.Select(tag => tag.Title).ToList()
        };
    }

    private static PromotionExtrasViewModel ServiceExtras(Service service) => new()
    {
        Benefits = (service.ServiceBenefits ?? [])
            .Select(benefit => benefit.BenefitDescription)
            .Where(description => !string.IsNullOrWhiteSpace(description))
            .ToList()
    };

    /// <summary>
    /// Price and stock are read through the same two rules the product listing uses, so a
    /// promotion card and a listing tile for the same product never disagree.
    /// </summary>
    /// <remarks>
    /// Only variants carry a price and a stock record of their own, but neither rule needs the
    /// caller to know that: <c>GetCatalogPrice</c> walks a parent to its first variant, and
    /// <c>GetListingStockForProduct</c> folds a parent's variants into one state. Both answer
    /// for a variant, a parent and the parent behind a product page alike.
    ///
    /// The price is the catalog price rather than the raw <c>ProductPriceSchemaPrice</c> the
    /// spec names, so a discounted product does not advertise two different prices on one page.
    /// It falls back to the schema price whenever no discount applies, and a product with no
    /// price at all comes back as zero, which the card omits rather than printing "$0.00".
    /// </remarks>
    private async Task<PromotionExtrasViewModel> ProductExtras(IProductSchema product)
    {
        decimal catalogPrice = await productService.GetCatalogPrice(product);
        var stockStatus = await productService.GetListingStockForProduct(product);

        return new PromotionExtrasViewModel
        {
            Price = catalogPrice > 0m ? catalogPrice : null,
            StockStatus = stockStatus == ProductStockEnum.Unknown ? null : stockStatus
        };
    }

    public ContentPromotionDisplayValues ResolveDisplayValues(
        ContentPromotionWidgetProperties properties,
        PromotedItemSource? item,
        AssetViewModel? overrideImage = null) => new()
        {
            Title = Resolve(properties, ContentPromotionElement.TITLE, properties.Title, item?.Title),
            DescriptionHtml = ResolveDescription(properties, item?.Description),
            CallToActionText = Resolve(properties, ContentPromotionElement.CALL_TO_ACTION, properties.CallToActionText, item?.CallToActionText),
            Image = ResolveImage(properties, overrideImage, item?.Image)
        };

    private static PromotedItemResult FromService(Service? service) => service is null
        ? new PromotedItemResult()
        : new PromotedItemResult
        {
            Family = ContentFamily.Service,
            PromotedContent = service,
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
    /// The description follows the same hide-override-inherit rule as every other element, but
    /// its two sources are not the same kind of value. An inherited description is rich text
    /// authored in the content hub and is passed through as markup; a typed override comes from
    /// a plain text area, so it is encoded and whatever the author typed shows up literally.
    /// </summary>
    /// <remarks>
    /// The cascade itself is <see cref="Resolve"/>'s, unchanged - only the two candidate values
    /// are prepared differently before it chooses between them. Preparing the override even when
    /// it loses costs an encode of a string the editor typed, which is not worth a second
    /// copy of the ordering rule to avoid.
    /// </remarks>
    private static HtmlString ResolveDescription(
        ContentPromotionWidgetProperties properties,
        string? inheritedValue) => new(Resolve(
            properties,
            ContentPromotionElement.DESCRIPTION,
            HtmlEncoder.Default.Encode(properties.Description),
            WithoutAnchors(inheritedValue)));

    /// <summary>
    /// Unwraps any anchors in an inherited description, keeping their text.
    /// </summary>
    /// <remarks>
    /// The whole card is one stretched link (spec section 6.3). An anchor inside the description
    /// sits underneath that link, so it cannot be clicked - but it is still reached by keyboard,
    /// leaving a focus stop that does nothing. A promotion card has exactly one destination, so
    /// the link is dropped and its text kept rather than the card losing its own clickability.
    ///
    /// This is not sanitisation and must not be read as any: the description is rendered as
    /// markup either way, and the content hub is the trust boundary. It removes one element that
    /// conflicts with the card's layout, nothing more.
    /// </remarks>
    private static string WithoutAnchors(string? description) =>
        string.IsNullOrWhiteSpace(description)
            ? string.Empty
            : AnchorTag().Replace(description, string.Empty);

    /// <summary>
    /// Matches an opening or closing anchor tag and nothing else. The word boundary after the
    /// <c>a</c> keeps <c>abbr</c>, <c>article</c> and <c>address</c> out of it, and quoted
    /// attribute values are matched as units so a legal <c>&gt;</c> inside one does not end the
    /// tag early and leave the rest of it on the page as text. An anchor written with an
    /// unquoted attribute value containing <c>&gt;</c>, or one shown as sample text inside a
    /// <c>code</c> block, is beyond what a pattern can tell apart.
    /// </summary>
    [GeneratedRegex("""</?a\b(?:[^>"']|"[^"]*"|'[^']*')*>""", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex AnchorTag();

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
