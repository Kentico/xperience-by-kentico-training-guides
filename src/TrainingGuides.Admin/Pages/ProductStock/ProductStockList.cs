using CMS.Base;
using CMS.Commerce;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using Kentico.Xperience.Admin.Base;
using TrainingGuides.Admin.ProductStock;
using TrainingGuides.ProductStock;

// Registers this page as the main listing page for the ProductStock application
[assembly: UIPage(typeof(ProductStockApplication), "list", typeof(ProductStockList), "List of product stock", TemplateNames.LISTING, UIPageOrder.First)]

namespace TrainingGuides.Admin.ProductStock;

// Product stock listing page that displays all stock records with product information
public sealed class ProductStockList(IContentLanguageRetriever contentLanguageRetriever, IProductQuantityFormatter productQuantityFormatter) : ListingPage
{
    // Specifies which object type this listing page manages
    protected override string ObjectType => ProductAvailableStockInfo.OBJECT_TYPE;

    public override async Task ConfigurePage()
    {
        // Gets the default language to ensure consistent data retrieval across multilingual content
        var defaultContentLanguage = await contentLanguageRetriever.GetDefaultContentLanguage();

        // Adds edit action for each row in the listing
        PageConfiguration.AddEditRowAction<ProductStockEditSection>();

        // Configures the columns that will be displayed in the listing grid
        PageConfiguration.ColumnConfigurations
                        // Product name comes from ContentItemLanguageMetadata table
                        .AddColumn("ContentItemLanguageMetadataDisplayName", "Product name", searchable: true)
                        // SKU comes from reusable field schema stored in CMS_ContentItemCommonData
                        .AddColumn(nameof(IProductSkuSchema.ProductSkuSchemaSkuCode), "SKU", searchable: true)
                        // Stock value from the ProductAvailableStockInfo table with custom formatting
                        .AddColumn(nameof(ProductAvailableStockInfo.ProductAvailableStockValue), "Stock", formatter: StockFormatter);

        // Joins necessary tables to retrieve product information
        // This is required because ProductStockInfo only contains ContentItemID, not product details
        PageConfiguration.QueryModifiers.AddModifier(query =>
            query.Source(
                s => s
                    // Joins with ContentItemLanguageMetadata to get product display names
                    .Join(
                        "CMS_ContentItemLanguageMetadata",
                        new WhereCondition($"[CMS_ContentItemLanguageMetadata].[ContentItemLanguageMetadataContentItemID] = [{ProductAvailableStockInfo.TYPEINFO.ClassStructureInfo.TableName}].[{nameof(ProductAvailableStockInfo.ProductAvailableStockContentItemID)}]")
                            // Filters by default language to avoid duplicate rows in multilingual setups
                            .And(new WhereCondition().WhereEquals("ContentItemLanguageMetadataContentLanguageID", defaultContentLanguage.ContentLanguageID))
                    )
                    // Joins with ContentItemCommonData to get reusable field schema data (like SKU)
                    .Join(
                        "CMS_ContentItemCommonData",
                        new WhereCondition($"[CMS_ContentItemCommonData].[ContentItemCommonDataContentItemID] = [{ProductAvailableStockInfo.TYPEINFO.ClassStructureInfo.TableName}].[{nameof(ProductAvailableStockInfo.ProductAvailableStockContentItemID)}]")
                            // Filters by default language for consistency
                            .And(new WhereCondition().WhereEquals("ContentItemCommonDataContentLanguageID", defaultContentLanguage.ContentLanguageID))
                    )
            )
        );

        await base.ConfigurePage();
    }

    // Formats the stock value for display using the commerce quantity formatter
    private string StockFormatter(object value, IDataContainer dataContainer) =>
        productQuantityFormatter.Format((decimal)value, new ProductQuantityFormatContext());
}