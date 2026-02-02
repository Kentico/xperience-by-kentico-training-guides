using CMS.Commerce;
using CMS.ContentEngine;
using TrainingGuides.Web.Features.Commerce.PriceCalculation.Models;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Commerce.Products.Services;

public class TrainingGuidesProductDataRetriever<TProductIdentifier, TProductData>(
    IContentItemRetrieverService contentItemRetrieverService) : IProductDataRetriever<TProductIdentifier, TProductData>
        where TProductIdentifier : TrainingGuidesPriceIdentifier
        where TProductData : TrainingGuidesProductData
{
    public async Task<IReadOnlyDictionary<TProductIdentifier, TProductData>> Get(
        IEnumerable<TProductIdentifier> productIdentifiers,
        string languageName,
        CancellationToken cancellationToken = default)
    {
        var products = await contentItemRetrieverService.RetrieveContentItemsBySchemas<IProductPriceSchema>(
            [IProductPriceSchema.REUSABLE_FIELD_SCHEMA_NAME],
            query => query.Where(where => where
                .WhereIn(nameof(ContentItemFields.ContentItemID), productIdentifiers.Select(x => x.Identifier))),
            2,
            true,
            languageName);

        var resultDictionary = new Dictionary<TProductIdentifier, TProductData>();

        foreach (var product in products)
        {
            if (product is IContentItemFieldsSource productItem)
            {
                var identifier = new TrainingGuidesPriceIdentifier
                {
                    Identifier = productItem.SystemFields.ContentItemID,
                };

                var productData = new TrainingGuidesProductData
                {
                    UnitPrice = product.ProductPriceSchemaPrice,
                    DiscountCategories = product.ProductPriceSchemaDiscountCategory ?? [],
                };

                resultDictionary.Add((TProductIdentifier)identifier, (TProductData)productData);

            }
        }
        return resultDictionary;
    }
}