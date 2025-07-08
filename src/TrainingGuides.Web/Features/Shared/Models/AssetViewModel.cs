namespace TrainingGuides.Web.Features.Shared.Models;

public class AssetViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public bool UseInternalOnly { get; set; }
    public string FilePath { get; set; } = string.Empty;

    public static AssetViewModel GetViewModel(Asset? asset) => asset is null
        ? new()
        : new()
        {
            Title = asset.AssetFile.Metadata.Name,
            Description = asset.AssetDescription,
            AltText = asset.AssetAltText,
            UseInternalOnly = asset.AssetUseInternalOnly,
            FilePath = asset.AssetFile.Url
        };
}
