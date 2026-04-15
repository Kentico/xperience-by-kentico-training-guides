using CMS;
using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using TrainingGuides.ProductStock;
using TrainingGuides.Web.Commerce.EventHandlers;


// Registers the custom module into the system
[assembly: RegisterModule(typeof(ProductStockCreationHandler))]

namespace TrainingGuides.Web.Commerce.EventHandlers;

// After we created this code sample, Xperience refresh 31.3.0 added a new way to implement and register content item event handlers,
// with full support for async/await code and dependency injection.
// See https://docs.kentico.com/documentation/developers-and-admins/customization/handle-global-events/reference-global-system-events#contentitemevents
public class ProductStockCreationHandler() : Module(MODULE_NAME)
{

    // We are setting this to default! to avoid a compiler warning.
    // We know it will be initialized in the OnInit method, instead of the typical constructor DI pattern.
    // This is a known limitation of the Module base class and does not indicate actual null safety issues in this code.
    private IInfoProvider<ProductAvailableStockInfo> productStockInfoProvider = default!;

    public const string MODULE_NAME = "Product stock creation handlers";

    // Contains initialization code that is executed when the application starts
    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit();

        productStockInfoProvider = parameters.Services.GetRequiredService<IInfoProvider<ProductAvailableStockInfo>>();

        // Assigns custom handlers to events
        // Suppress CS8622: Kentico's event system delegates have nullability attribute mismatches with our handler signatures.
        // This is a known framework limitation and does not indicate actual null safety issues in this code.
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
        ContentItemEvents.Create.After += ContentItem_Create_After;
        ContentItemEvents.UpdateDraft.Before += ContentItem_UpdateDraft_Before;
        ContentItemEvents.Delete.Execute += ContentItem_Delete_Execute;
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

    private void ContentItem_Delete_Execute(object? sender, DeleteContentItemEventArgs e)
    {
        if (!IsStockKeepingItem(e.ContentTypeName))
            return;
        var existingProductStock = GetExistingStockRecords(e.ID);

        foreach (var availableStock in existingProductStock)
        {
            availableStock.Delete();
        }
    }

    private void EnsureStockRecord(int contentItemId, string skuCode = "")
    {
        var existingProductStock = GetExistingStockRecords(contentItemId);

        if (existingProductStock.Any())
        {
            foreach (var availableStock in existingProductStock)
            {
                // Update the sku code in case it has changed
                availableStock.ProductAvailableStockSKUCode = skuCode;
                productStockInfoProvider.Set(availableStock);
            }
        }
        else
        {
            productStockInfoProvider.Set(new ProductAvailableStockInfo
            {
                ProductAvailableStockContentItemID = contentItemId,
                ProductAvailableStockValue = 0,
                ProductAvailableStockSKUCode = skuCode,
                ProductAvailableStockGUID = Guid.NewGuid()
            });
        }
    }

    private bool IsStockKeepingItem(string contentTypeName)
    {
        var types = GetProductContentTypeNames();
        return types.Contains(contentTypeName);
    }

    private IEnumerable<ProductAvailableStockInfo> GetExistingStockRecords(int contentItemId) =>
        productStockInfoProvider.Get()
            .WhereEquals(nameof(ProductAvailableStockInfo.ProductAvailableStockContentItemID), contentItemId)
            .GetEnumerableTypedResult();

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