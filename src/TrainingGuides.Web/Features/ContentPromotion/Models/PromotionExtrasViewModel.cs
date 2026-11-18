using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Commerce.Products.Models;

namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// Read-only facts that only one content family has. Empty is a valid state: the block is
/// simply not rendered.
/// </summary>
public class PromotionExtrasViewModel
{
    /// <summary>
    /// Article family: the display names of the item's categories.
    /// </summary>
    public IReadOnlyList<string> Categories { get; set; } = [];

    /// <summary>
    /// Service family: the descriptions of the service's benefits, as markup - they are rich text.
    /// </summary>
    public IReadOnlyList<HtmlString> Benefits { get; set; } = [];

    /// <summary>
    /// Product family: the catalog price. For a parent product, the price of its first variant.
    /// Null when the product has no price.
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Product family: the stock state, folded across variants for a parent product. Null when no
    /// stock record exists; zero stock is <see cref="ProductStockEnum.OutOfStock"/>.
    /// </summary>
    public ProductStockEnum? StockStatus { get; set; }

    public bool HasContent =>
        Categories.Count > 0
        || Benefits.Count > 0
        || Price.HasValue
        || StockStatus.HasValue;
}
