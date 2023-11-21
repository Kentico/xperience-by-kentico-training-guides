namespace TrainingGuides.Web.Models;

public class AssetViewModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string AltText { get; set; }
    public bool UseInternalOnly { get; set; }
    public string FilePath { get; set; }

    public static AssetViewModel GetViewModel(Asset asset)
    {
        return new AssetViewModel()
        {
            Title = asset.AssetFile.Metadata.Name,
            Description = asset.AssetDescription,
            AltText = asset.AssetAltText,
            UseInternalOnly = asset.AssetUseInternalOnly,
            FilePath = asset.AssetFile?.Url
        };
    }
}