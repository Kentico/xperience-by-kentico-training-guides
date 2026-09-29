using TrainingGuides.Web.Commerce.Products.Models;

namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// The read-only, never overridable block of facts that only one content family has.
/// Every member is empty when the family has nothing to show, which is a valid state -
/// the block simply disappears (spec section 7.1).
/// </summary>
public class PromotionExtrasViewModel
{
    /// <summary>
    /// Article family: the display names of the item's categories.
    /// </summary>
    public IEnumerable<string> Categories { get; set; } = [];

    /// <summary>
    /// Service family: the descriptions of the service's benefits.
    /// </summary>
    public IEnumerable<string> Benefits { get; set; } = [];

    /// <summary>
    /// Product family: the price of the selected variant. Null whenever the selected item
    /// does not carry a price of its own - a parent product or a product page.
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Product family: the stock state of the selected variant. Null when no stock record
    /// exists. Zero stock is <see cref="ProductStockEnum.OutOfStock"/> rather than null,
    /// because out of stock is worth saying out loud.
    /// </summary>
    public ProductStockEnum? StockStatus { get; set; }

    public bool HasContent =>
        Categories.Any()
        || Benefits.Any()
        || Price.HasValue
        || StockStatus.HasValue;
}
