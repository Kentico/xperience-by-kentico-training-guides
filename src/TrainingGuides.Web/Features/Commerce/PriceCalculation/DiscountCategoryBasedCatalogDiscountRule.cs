using CMS.Commerce;
using Kentico.Xperience.Admin.DigitalCommerce;
using TrainingGuides.Web.Features.Commerce.PriceCalculation.Models;

[assembly: RegisterPromotionRule<TrainingGuides.Web.Features.Commerce.PriceCalculation.DiscountCategoryBasedCatalogDiscountRule>(
    identifier: "DiscountCategoryBasedCatalogDiscount",
    promotionType: PromotionType.Catalog,
    name: "Discount based on product discount category")]

namespace TrainingGuides.Web.Features.Commerce.PriceCalculation;

/// <summary>
/// Catalog promotion rule that applies discounts to products based on their assigned discount categories.
/// Uses <see cref="TrainingGuidesProductData"/> to access product discount category information.
/// </summary>
public class DiscountCategoryBasedCatalogDiscountRule
    : CatalogPromotionRule<DiscountCategoryBasedCatalogDiscountProperties,
                           TrainingGuidesPriceIdentifier,
                           PriceCalculationRequest,
                           TrainingGuidesPriceCalculationResult>
{
    /// <summary>
    /// Evaluates whether the given product is eligible for this catalog discount
    /// and calculates the discount amount.
    /// </summary>
    /// <param name="productIdentifier">The identifier of the product to evaluate.</param>
    /// <param name="calculationData">The price calculation context containing product data.</param>
    /// <returns>A <see cref="CatalogPromotionCandidate"/> if the product is eligible; otherwise, null.</returns>
    public override CatalogPromotionCandidate? GetPromotionCandidate(
        TrainingGuidesPriceIdentifier productIdentifier,
        IPriceCalculationData<PriceCalculationRequest, TrainingGuidesPriceCalculationResult> calculationData)
    {
        // Gets the result item for the current product
        var resultItem = calculationData.Result.Items
            .FirstOrDefault(i => i.ProductIdentifier == productIdentifier);

        if (resultItem?.ProductData is not TrainingGuidesProductData productData)
        {
            return null;
        }

        // Checks if the product belongs to any of the discount categories
        // selected when creating the promotion rule
        // Compare by Identifier (Guid) instead of TagReference equality
        bool isInEligibleCategory = productData.DiscountCategories
            .Select(dc => dc.Identifier)
            .Intersect(Properties.DiscountCategories.Select(pc => pc.Identifier))
            .Any();

        if (!isInEligibleCategory)
        {
            return null;
        }

        // Calculates the discount using the built-in helper method
        // Handles both percentage and fixed discounts based on DiscountValueType
        decimal discountAmount = GetDiscountAmount(productData.UnitPrice);

        return new CatalogPromotionCandidate
        {
            UnitPriceDiscountAmount = discountAmount
        };
    }
}
