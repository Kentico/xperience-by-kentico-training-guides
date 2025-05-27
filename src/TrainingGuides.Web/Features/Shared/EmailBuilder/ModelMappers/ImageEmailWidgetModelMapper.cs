

using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.ModelMappers;

public class ImageEmailWidgetModelMapper : IComponentModelMapper<ImageWidgetModel>
{
    private readonly IContentItemRetrieverService<Asset> assetRetrieverService;
    private readonly IContentItemRetrieverService<GalleryImage> galleryImageRetrieverService;

    public ImageEmailWidgetModelMapper(
        IContentItemRetrieverService<Asset> assetRetrieverService,
        IContentItemRetrieverService<GalleryImage> galleryImageRetrieverService)
    {
        this.assetRetrieverService = assetRetrieverService;
        this.galleryImageRetrieverService = galleryImageRetrieverService;
    }


    public async Task<ImageWidgetModel> Map(Guid itemGuid, string languageName)
    {
        var asset = await assetRetrieverService.RetrieveContentItemByGuid(itemGuid, Asset.CONTENT_TYPE_NAME, languageName: languageName);
        var galleryImage = await galleryImageRetrieverService.RetrieveContentItemByGuid(itemGuid, GalleryImage.CONTENT_TYPE_NAME);

        if (asset is null && galleryImage is null)
        {
            return new ImageWidgetModel();
        }

        return new ImageWidgetModel()
        {
            // Populates the image URL and alt text from the retrieved content item's fields
            ImageUrl = asset?.AssetFile.Url
                ?? galleryImage?.GalleryImageAsset?.FirstOrDefault()?.AssetFile?.Url
                ?? string.Empty,
            AltText = asset?.AssetAltText
                ?? galleryImage?.GalleryImageAsset?.FirstOrDefault()?.AssetAltText
                ?? string.Empty
        };
    }
}