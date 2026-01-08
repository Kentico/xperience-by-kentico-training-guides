using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.Mappers;

public class ProductEmailWidgetModelMapper : IComponentModelMapper<ProductWidgetModel>
{

    private readonly IContentItemRetrieverService contentItemRetrieverService;

    public ProductEmailWidgetModelMapper(
        IContentItemRetrieverService contentItemRetrieverService)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
    }

    public async Task<ProductWidgetModel> Map(Guid webPageItemContentItemGuid, string languageName)
    {
        var page = await contentItemRetrieverService.RetrieveWebPageByContentItemGuid<ServicePage>(
            contentItemGuid: webPageItemContentItemGuid,
            depth: 2,
            languageName: languageName);

        var service = page?.ServicePageService.FirstOrDefault();

        // If the service or page is null, return an empty model. Note the service will always be null if the page is null.
        if (service is null)
        {
            return new ProductWidgetModel();
        }

        string webPageItemUrl = page.GetUrl().AbsoluteUrl;

        var image = service.ServiceMedia.FirstOrDefault();
        string imageUrl = image?.AssetFile?.Url ?? string.Empty;

        return new ProductWidgetModel
        {
            Name = service.ServiceName,
            Description = service.ServiceDescription,
            Url = webPageItemUrl,
            ImageUrl = imageUrl,
            ImageAltText = image?.AssetAltText ?? image?.AssetDescription ?? string.Empty,
        };
    }
}