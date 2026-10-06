using System.Text.Encodings.Web;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
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

public class ContentPromotionService(
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
        if (properties.IsContentSource(ContentPromotionSource.MANUAL))
        {
            return new PromotedItemResult();
        }

        bool fromContentHub = properties.IsContentSource(ContentPromotionSource.CONTENT_ITEM);

        var selectedGuid = (fromContentHub ? properties.SelectedContentItem : properties.SelectedPage)
            .Select(selected => selected.Identifier)
            .FirstOrDefault();

        if (selectedGuid == Guid.Empty)
        {
            return new PromotedItemResult();
        }

        // Naming the selector's content types is what makes their type-specific fields available.
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

        // Something was selected, so nothing usable coming back (for example, a page whose linked
        // item is missing) is a broken selection, not an empty one.
        result.SelectionFailed = result.Item is null;

        // Lets the link rules reuse the page instead of retrieving it again.
        result.Page = item as IWebPageFieldsSource;

        return result;
    }

    public ContentPromotionDisplayValues ResolveDisplayValues(
        ContentPromotionWidgetProperties properties,
        PromotedItemSource? item,
        AssetViewModel? overrideImage = null) => new()
        {
            Title = Resolve(properties, ContentPromotionElement.TITLE, properties.Title, item?.Title),
            DescriptionHtml = ResolveDescription(properties, item?.Description),
            CallToActionText = Resolve(properties, ContentPromotionElement.CALL_TO_ACTION, properties.CallToActionText, null),
            Image = ResolveImage(properties, overrideImage, item?.Image)
        };

    public async Task<AssetViewModel?> ResolveOverrideImage(ContentPromotionWidgetProperties properties)
    {
        // A hidden image would be thrown away, so skip the query.
        if (properties.IsElementHidden(ContentPromotionElement.IMAGE))
        {
            return null;
        }

        var imageGuid = properties.Image.Select(image => image.Identifier).FirstOrDefault();

        return imageGuid == Guid.Empty
            ? null
            : GetImage(await contentItemRetrieverService.RetrieveContentItemByGuid<Asset>(imageGuid));
    }

    public async Task<LinkViewModel?> ResolveLink(
        ContentPromotionWidgetProperties properties,
        IWebPageFieldsSource? selectedPage)
    {
        bool pageMode = properties.IsContentSource(ContentPromotionSource.PAGE);

        var destinationPage = pageMode
            ? selectedPage
            : await RetrieveLinkTargetPage(properties);

        // A target page wins over a typed URL. Hiding a form field does not clear its stored value,
        // so a typed URL only counts where the form shows it - never in page mode.
        string url = destinationPage is not null
            ? (await webPageUrlRetriever.Retrieve(destinationPage)).RelativePath
            : pageMode ? string.Empty : properties.LinkUrl;

        return string.IsNullOrWhiteSpace(url)
            ? null
            : new LinkViewModel
            {
                LinkUrl = url,
                OpenInNewTab = properties.OpenInNewTab
            };
    }

    public async Task<PromotionExtrasViewModel> ResolveExtras(
        ContentPromotionWidgetProperties properties,
        PromotedItemResult promotedItem,
        CancellationToken cancellationToken = default)
    {
        if (!properties.ShowExtras)
        {
            return new PromotionExtrasViewModel();
        }

        return promotedItem.PromotedContent switch
        {
            IArticleSchema article => await ArticleExtras(article),
            Service service => ServiceExtras(service),
            IProductSchema product => await ProductExtras(product, cancellationToken),
            _ => new PromotionExtrasViewModel()
        };
    }

    // Content family mappers

    private static PromotedItemResult FromArticle(IArticleSchema? article) => article is null
        ? new PromotedItemResult()
        : new PromotedItemResult
        {
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
            PromotedContent = product,
            Item = new PromotedItemSource
            {
                Title = product.ProductSchemaName,
                Description = product.ProductSchemaDescription,
                Image = GetProductImage(product)
            }
        };

    private static PromotedItemResult FromService(Service? service) => service is null
        ? new PromotedItemResult()
        : new PromotedItemResult
        {
            PromotedContent = service,
            Item = new PromotedItemSource
            {
                Title = service.ServiceName,
                Description = service.ServiceShortDescription,
                Image = GetImage(service.ServiceMedia?.FirstOrDefault())
            }
        };

    // Images

    /// <summary>
    /// Returns null for a missing asset. <see cref="AssetViewModel.GetViewModel"/> returns an empty
    /// model instead, which would render an <c>img</c> with no source.
    /// </summary>
    private static AssetViewModel? GetImage(Asset? asset)
    {
        var image = AssetViewModel.GetViewModel(asset);

        return string.IsNullOrWhiteSpace(image.FilePath) ? null : image;
    }

    /// <summary>
    /// The first image of the product or, when a parent product has none of its own, of its
    /// variants - the same fallback the product page and listing use.
    /// </summary>
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
    /// Products keep images in <c>ProductImage</c> items rather than assets. Like
    /// <see cref="GetImage(Asset?)"/>, an image with no file behind it is no image.
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

    // Resolution rules: hide beats override, override beats inherit

    private static string Resolve(
        ContentPromotionWidgetProperties properties,
        string element,
        string overrideValue,
        string? inheritedValue)
    {
        if (properties.IsElementHidden(element))
        {
            return string.Empty;
        }

        return string.IsNullOrWhiteSpace(overrideValue)
            ? inheritedValue ?? string.Empty
            : overrideValue;
    }

    /// <summary>
    /// Same rule as the other elements, but the two sources differ: an inherited description is
    /// rich text and stays markup, while the typed override comes from a plain text area and is
    /// encoded, so whatever the editor typed shows up literally.
    /// </summary>
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
    /// The whole card is one stretched link, so an anchor inside it cannot be clicked but is still a
    /// keyboard focus stop. This is not sanitisation - the content hub is the trust boundary. A
    /// description with no anchor is returned untouched, because parsing re-serializes the markup.
    /// </remarks>
    private static string WithoutAnchors(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return string.Empty;
        }

        var body = new HtmlParser().ParseDocument(string.Empty).Body!;
        body.InnerHtml = description;

        var anchors = body.QuerySelectorAll("a").ToList();

        if (anchors.Count == 0)
        {
            return description;
        }

        foreach (var anchor in anchors)
        {
            anchor.Replace([.. anchor.ChildNodes]);
        }

        return body.InnerHtml;
    }

    /// <summary>
    /// The override arrives already resolved, because resolving it needs content retrieval.
    /// </summary>
    private static AssetViewModel? ResolveImage(
        ContentPromotionWidgetProperties properties,
        AssetViewModel? overrideImage,
        AssetViewModel? inheritedImage)
    {
        if (properties.IsElementHidden(ContentPromotionElement.IMAGE))
        {
            return null;
        }

        return overrideImage ?? inheritedImage;
    }

    // Link and extras lookups

    private async Task<IWebPageFieldsSource?> RetrieveLinkTargetPage(ContentPromotionWidgetProperties properties)
    {
        var targetGuid = properties.LinkTargetPage.Select(page => page.Identifier).FirstOrDefault();

        return targetGuid == Guid.Empty
            ? null
            : await contentItemRetrieverService.RetrieveWebPageForUrlByContentItemGuid(targetGuid);
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
            Categories = [.. tags.Select(tag => tag.Title)]
        };
    }

    private static PromotionExtrasViewModel ServiceExtras(Service service) => new()
    {
        Benefits =
        [
            .. (service.ServiceBenefits ?? [])
                .Select(benefit => benefit.BenefitDescription)
                .Where(description => !string.IsNullOrWhiteSpace(description))
        ]
    };

    /// <summary>
    /// Price and stock come from the same two rules the product listing uses, so the card and the
    /// listing never disagree. Both work for a variant and for a parent product.
    /// </summary>
    /// <remarks>
    /// The price is the catalog price, after any discount, not the raw schema price - otherwise a
    /// discounted product would show two prices on one page. No price comes back as zero, which the
    /// card omits rather than showing a zero price.
    /// </remarks>
    private async Task<PromotionExtrasViewModel> ProductExtras(IProductSchema product, CancellationToken cancellationToken)
    {
        decimal catalogPrice = await productService.GetCatalogPrice(product, cancellationToken);
        var stockStatus = await productService.GetListingStockForProduct(product);

        return new PromotionExtrasViewModel
        {
            Price = catalogPrice > 0m ? catalogPrice : null,
            StockStatus = stockStatus == ProductStockEnum.Unknown ? null : stockStatus
        };
    }
}
