using CMS;
using CMS.Base;
using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;

using TrainingGuides.Web.Commerce.EventHandlers;


// Registers the custom module into the system
[assembly: RegisterModule(typeof(ProductStockModule))]

namespace TrainingGuides.Web.Commerce.EventHandlers;

public class ProductStockModule : Module
{
    public ProductStockModule() : base(nameof(ProductStockModule)) { }

    protected override void OnPreInit(ModulePreInitParameters parameters)
    {
        base.OnPreInit(parameters);

        parameters.Services.AddSingleton<ProductStockCreationService>();
        parameters.Services.AddEventHandler<AfterCreateContentItemEvent, ProductStockAfterCreateHandler>();
        parameters.Services.AddEventHandler<BeforeUpdateDraftEvent, ProductStockBeforeUpdateDraftHandler>();
        parameters.Services.AddEventHandler<AfterDeleteContentItemEvent, ProductStockAfterDeleteHandler>();
    }
}

internal class ProductStockAfterCreateHandler(ProductStockCreationService service) : IAsyncEventHandler<AfterCreateContentItemEvent>
{
    public async Task HandleAsync(AfterCreateContentItemEvent asyncEvent, CancellationToken cancellationToken)
    {
        var data = asyncEvent.Data;

        if (data.ID is null || !service.IsStockKeepingItem(data.ContentTypeName))
            return;

        data.ContentItemData.TryGetValue(nameof(IProductSkuSchema.ProductSkuSchemaSkuCode), out string skuCode);
        await service.EnsureStockRecord(data.ID.Value, skuCode, cancellationToken);
    }
}

internal class ProductStockBeforeUpdateDraftHandler(ProductStockCreationService service) : IAsyncEventHandler<BeforeUpdateDraftEvent>
{
    public async Task HandleAsync(BeforeUpdateDraftEvent asyncEvent, CancellationToken cancellationToken)
    {
        var data = asyncEvent.Data;

        if (!service.IsStockKeepingItem(data.ContentTypeName))
            return;

        data.ContentItemData.TryGetValue(nameof(IProductSkuSchema.ProductSkuSchemaSkuCode), out string skuCode);
        await service.EnsureStockRecord(data.ID, skuCode, cancellationToken);
    }
}

internal class ProductStockAfterDeleteHandler(ProductStockCreationService service) : IAsyncEventHandler<AfterDeleteContentItemEvent>
{
    public async Task HandleAsync(AfterDeleteContentItemEvent asyncEvent, CancellationToken cancellationToken)
    {
        var data = asyncEvent.Data;

        if (!service.IsStockKeepingItem(data.ContentTypeName))
            return;

        await service.DeleteStockRecords(data.ID, cancellationToken);
    }
}