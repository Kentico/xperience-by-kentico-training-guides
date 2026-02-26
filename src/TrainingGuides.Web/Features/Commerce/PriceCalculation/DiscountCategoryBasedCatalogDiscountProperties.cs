using CMS.ContentEngine;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.DigitalCommerce;

namespace TrainingGuides.Web.Features.Commerce.PriceCalculation;

/// <summary>
/// Properties for configuring a catalog discount based on product discount category.
/// Extends <see cref="CatalogPromotionRuleProperties"/> to include standard discount fields.
/// </summary>
public class DiscountCategoryBasedCatalogDiscountProperties : CatalogPromotionRuleProperties
{
    // DiscountValue and DiscountValueType properties
    // are inherited from CatalogPromotionRuleProperties

    /// <summary>
    /// Discount categories eligible for the discount.
    /// </summary>
    [TagSelectorComponent(
        "ProductDiscountCategory",
        Label = "Discount categories",
        Order = 1)]
    public IEnumerable<TagReference> DiscountCategories { get; set; } = [];
}
