using CMS.DataEngine;
using TrainingGuides.ProductStock;


namespace TrainingGuides.Web.Commerce.EventHandlers;

internal class ProductStockCreationService(IInfoProvider<ProductAvailableStockInfo> productStockInfoProvider)
{
    private readonly HashSet<string> stockKeepingTypeNames = GetProductContentTypeNames();

    internal bool IsStockKeepingItem(string contentTypeName) =>
        stockKeepingTypeNames.Contains(contentTypeName);

    internal async Task EnsureStockRecord(int contentItemId, string skuCode, CancellationToken cancellationToken)
    {
        var existingProductStock = await GetExistingStockRecords(contentItemId, cancellationToken);

        if (existingProductStock.Any())
        {
            foreach (var availableStock in existingProductStock)
            {
                // Update the SKU code in case it has changed
                availableStock.ProductAvailableStockSKUCode = skuCode;
                await productStockInfoProvider.SetAsync(availableStock, cancellationToken);
            }
        }
        else
        {
            await productStockInfoProvider.SetAsync(new ProductAvailableStockInfo
            {
                ProductAvailableStockContentItemID = contentItemId,
                ProductAvailableStockValue = 0,
                ProductAvailableStockSKUCode = skuCode,
                ProductAvailableStockGUID = Guid.NewGuid()
            }, cancellationToken);
        }
    }

    internal async Task DeleteStockRecords(int contentItemId, CancellationToken cancellationToken)
    {
        var existingProductStock = await GetExistingStockRecords(contentItemId, cancellationToken);

        foreach (var availableStock in existingProductStock)
        {
            availableStock.Delete();
        }
    }

    private async Task<IEnumerable<ProductAvailableStockInfo>> GetExistingStockRecords(int contentItemId, CancellationToken cancellationToken) => await productStockInfoProvider.Get()
            .WhereEquals(nameof(ProductAvailableStockInfo.ProductAvailableStockContentItemID), contentItemId)
            .GetEnumerableTypedResultAsync(cancellationToken: cancellationToken);

    private static HashSet<string> GetProductContentTypeNames()
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
        .Where(name => name is not null)
        .Cast<string>()
        .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}