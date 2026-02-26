using CMS.Commerce;
using CMS.ContentEngine;

namespace TrainingGuides.Web.Features.Commerce.PriceCalculation.Models;

public record TrainingGuidesProductData : ProductData
{
    /// <summary>
    /// Discount categories assigned to the product via ProductPriceSchemaDiscountCategory.
    /// </summary>
    public IEnumerable<TagReference> DiscountCategories { get; init; } = [];
}