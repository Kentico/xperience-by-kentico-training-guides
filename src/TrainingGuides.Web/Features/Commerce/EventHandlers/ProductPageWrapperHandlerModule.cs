using CMS;
using CMS.Base;
using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;

using TrainingGuides.Web.Commerce.EventHandlers;

[assembly: RegisterModule(typeof(ProductPageWrapperHandlerModule))]

namespace TrainingGuides.Web.Commerce.EventHandlers;

public class ProductPageWrapperHandlerModule : Module
{
    public ProductPageWrapperHandlerModule() : base(nameof(ProductPageWrapperHandlerModule)) { }

    protected override void OnPreInit(ModulePreInitParameters parameters)
    {
        base.OnPreInit(parameters);

        parameters.Services.AddSingleton<ProductPageWrapperService>();
        parameters.Services.AddEventHandler<AfterCreateContentItemEvent, ProductPageCreateHandler>();
        parameters.Services.AddEventHandler<AfterCreateLanguageVariantEvent, ProductPageCreateLanguageVariantHandler>();
        parameters.Services.AddEventHandler<AfterDeleteContentItemEvent, ProductPageDeleteHandler>();
        parameters.Services.AddEventHandler<AfterPublishContentItemEvent, ProductPagePublishHandler>();
        parameters.Services.AddEventHandler<AfterUnpublishContentItemEvent, ProductPageUnpublishHandler>();
    }
}

// Handler triggered after a new product content item is created
internal class ProductPageCreateHandler(ProductPageWrapperService service) : IAsyncEventHandler<AfterCreateContentItemEvent>
{
    public async Task HandleAsync(AfterCreateContentItemEvent asyncEvent, CancellationToken cancellationToken)
    {
        var data = asyncEvent.Data;

        if (data.ID is null || !service.IsApplicableType(data.ContentTypeName))
            return;

        if (data.Guid is not null)
        {
            await service.CreatePageWrapperForProduct(
                displayName: data.DisplayName,
                languageName: data.ContentLanguageName,
                languageId: data.ContentLanguageID,
                contentTypeId: data.ContentTypeID,
                contentItemGuid: data.Guid.Value);
        }
    }
}

// Handler triggered after a new language variant of a product content item is created
internal class ProductPageCreateLanguageVariantHandler(ProductPageWrapperService service) : IAsyncEventHandler<AfterCreateLanguageVariantEvent>
{
    public async Task HandleAsync(AfterCreateLanguageVariantEvent asyncEvent, CancellationToken cancellationToken)
    {
        var data = asyncEvent.Data;

        if (!service.IsApplicableType(data.ContentTypeName))
            return;

        await service.EnsureProductPageWrapperForLanguage(
            displayName: data.DisplayName,
            languageName: data.ContentLanguageName,
            languageId: data.ContentLanguageID,
            contentTypeId: data.ContentTypeID,
            contentItemGuid: data.Guid);
    }
}

// Handler triggered after a product content item is deleted
internal class ProductPageDeleteHandler(ProductPageWrapperService service) : IAsyncEventHandler<AfterDeleteContentItemEvent>
{
    public async Task HandleAsync(AfterDeleteContentItemEvent asyncEvent, CancellationToken cancellationToken)
    {
        var data = asyncEvent.Data;

        if (!service.IsApplicableType(data.ContentTypeName))
            return;

        // If there is no existing page in the language being deleted, we don't need to create a new one, so we will not use the ensure method.
        await service.DeleteProductPageWrappers(
            guid: data.Guid,
            languageName: data.ContentLanguageName,
            languageId: data.ContentLanguageID);
    }
}

// Handler triggered after a product content item is published
internal class ProductPagePublishHandler(ProductPageWrapperService service) : IAsyncEventHandler<AfterPublishContentItemEvent>
{
    public async Task HandleAsync(AfterPublishContentItemEvent asyncEvent, CancellationToken cancellationToken)
    {
        var data = asyncEvent.Data;

        if (!service.IsApplicableType(data.ContentTypeName))
            return;

        await service.PublishProductPageWrappers(
            displayName: data.DisplayName,
            languageName: data.ContentLanguageName,
            languageId: data.ContentLanguageID,
            contentTypeId: data.ContentTypeID,
            contentItemGuid: data.Guid,
            contentItemId: data.ID);
    }
}

// Handler triggered after a product content item is unpublished
internal class ProductPageUnpublishHandler(ProductPageWrapperService service) : IAsyncEventHandler<AfterUnpublishContentItemEvent>
{
    public async Task HandleAsync(AfterUnpublishContentItemEvent asyncEvent, CancellationToken cancellationToken)
    {
        var data = asyncEvent.Data;

        if (!service.IsApplicableType(data.ContentTypeName))
            return;

        await service.UnpublishProductPageWrappers(
            contentItemGuid: data.Guid,
            languageName: data.ContentLanguageName,
            languageId: data.ContentLanguageID,
            contentItemId: data.ID);
    }
}

