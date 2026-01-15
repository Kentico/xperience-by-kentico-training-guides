using CMS;
using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using TrainingGuides.ProductStock;
using TrainingGuides.Web.Commerce.EventHandlers;


// Registers the custom module into the system
[assembly: RegisterModule(typeof(ProductStockCreationHandler))]

namespace TrainingGuides.Web.Commerce.EventHandlers;

public class ProductStockCreationHandler() : Module(MODULE_NAME)
{

    private IInfoProvider<ProductAvailableStockInfo> productStockInfoProvider;

    public const string MODULE_NAME = "Product stock creation handlers";

    // Contains initialization code that is executed when the application starts
    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit();

        productStockInfoProvider = parameters.Services.GetRequiredService<IInfoProvider<ProductAvailableStockInfo>>();

        // Assigns custom handlers to events
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
        ContentItemEvents.Create.After += ContentItem_Create_After;
        ContentItemEvents.UpdateDraft.Before += ContentItem_UpdateDraft_Before;
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
    }

    private void ContentItem_Create_After(object sender, CreateContentItemEventArgs e)
    {
        if (e.ID is null || !IsStockKeepingItem(e.ContentTypeName))
            return;

        e.ContentItemData.TryGetValue(nameof(IProductSkuSchema.ProductSkuSchemaSkuCode), out string skuCode);
        EnsureStockRecord((int)e.ID, skuCode);
    }

    private void ContentItem_UpdateDraft_Before(object sender, UpdateContentItemDraftEventArgs e)
    {
        if (!IsStockKeepingItem(e.ContentTypeName))
            return;

        e.ContentItemData.TryGetValue(nameof(IProductSkuSchema.ProductSkuSchemaSkuCode), out string skuCode);
        EnsureStockRecord(e.ID, skuCode);
    }

    private void EnsureStockRecord(int contentItemId, string skuCode = "")
    {
        var existingProductStock = productStockInfoProvider.Get()
            .WhereEquals(nameof(ProductAvailableStockInfo.ProductAvailableStockContentItemID), contentItemId)
            .GetEnumerableTypedResult();

        if (existingProductStock.Any())
        {
            foreach (var availableStock in existingProductStock)
            {
                availableStock.ProductStockSKUCode = skuCode;
                productStockInfoProvider.Set(availableStock);
            }
        }
        else
        {
            productStockInfoProvider.Set(new ProductAvailableStockInfo
            {
                ProductAvailableStockContentItemID = contentItemId,
                ProductAvailableStockValue = 0,
                ProductStockSKUCode = skuCode
            });
        }
    }

    private bool IsStockKeepingItem(string contentTypeName)
    {
        var types = GetProductContentTypeNames();
        return types.Contains(contentTypeName);
    }

    private IEnumerable<string> GetProductContentTypeNames()
    {
        // We know this class is stored in the Entities project, so we can use it to access the assembly
        var entitiesAssembly = typeof(ProductAvailableStockInfo).Assembly;
        var types = entitiesAssembly
            .GetTypes()
            .Where(type =>
                type.IsClass
                && !type.IsAbstract
                && typeof(IProductSkuSchema).IsAssignableFrom(type));

        return types.Select(type =>
        {
            var field = type.GetField("CONTENT_TYPE_NAME");
            if (field is not null)
            {
                // The field is a constant, so we don't need an instance to retrieve its value - we can use null instead
                return field.GetValue(null) as string;
            }
            // If there is no CONTENT_TYPE_NAME constant, we skip this type
            return null;
        })
        // Filter out the classes with no CONTENT_TYPE_NAME constant
        .Where(name => name is not null)!;
    }
}