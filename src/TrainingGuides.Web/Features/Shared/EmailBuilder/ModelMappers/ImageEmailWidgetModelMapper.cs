using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.ModelMappers;

public class ImageEmailWidgetModelMapper : IComponentModelMapper<ImageWidgetModel>
{
    private readonly IContentItemRetrieverService<Asset> assetRetrieverService;

    public ImageEmailWidgetModelMapper(
        IContentItemRetrieverService<Asset> assetRetrieverService)
    {
        this.assetRetrieverService = assetRetrieverService;
    }

    public async Task<ImageWidgetModel> Map(Guid itemGuid, string languageName)
    {
        var asset = await assetRetrieverService.RetrieveContentItemByGuid(
                contentItemGuid: itemGuid,
                contentTypeName: Asset.CONTENT_TYPE_NAME,
                languageName: languageName);

        if (asset is null)
        {
            return new ImageWidgetModel();
        }

        string imageUrl = asset.AssetFile?.Url ?? string.Empty;

        return new ImageWidgetModel()
        {
            // Populate the image URL and alt text from the retrieved content item's fields
            ImageUrl = imageUrl,
            AltText = asset?.AssetAltText ?? string.Empty
        };
    }
}