namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// Code names of the site's image variant definitions.
/// Each value must match its definition's code name exactly, because the keys of
/// <see cref="CMS.ContentEngine.ContentItemAsset.VariantUrls"/> are case-sensitive.
/// </summary>
public static class ImageVariants
{
    /// <summary>Scaled variant, 480 px wide, with the height in the original's aspect ratio.</summary>
    public const string SCALE_W480 = "ScaleW480";

    /// <summary>Scaled variant, 828 px wide, with the height in the original's aspect ratio.</summary>
    public const string SCALE_W828 = "ScaleW828";

    /// <summary>Scaled variant, 1280 px wide, with the height in the original's aspect ratio.</summary>
    public const string SCALE_W1280 = "ScaleW1280";

    /// <summary>Scaled variant, 1600 px wide, with the height in the original's aspect ratio.</summary>
    public const string SCALE_W1600 = "ScaleW1600";

    /// <summary>Scaled variant, 1920 px wide, with the height in the original's aspect ratio.</summary>
    public const string SCALE_W1920 = "ScaleW1920";

    /// <summary>Scaled variant, 2560 px wide, with the height in the original's aspect ratio.</summary>
    public const string SCALE_W2560 = "ScaleW2560";

    /// <summary>Smart crop variant for cards, 800 × 600 px (4:3).</summary>
    public const string CARD_800X600 = "Card800x600";

    /// <summary>Smart crop variant for heroes on phones, 828 × 828 px (1:1).</summary>
    public const string HERO_MOBILE_828X828 = "HeroMobile828x828";

    /// <summary>Smart crop variant for heroes on wide screens, 1920 × 1080 px (16:9).</summary>
    public const string HERO_WIDE_1920X1080 = "HeroWide1920x1080";

    /// <summary>Smart crop variant for thumbnails, 256 × 256 px (1:1).</summary>
    public const string THUMBNAIL_256X256 = "Thumbnail256x256";

    /// <summary>
    /// Code names of the scaled variants, narrowest first.
    /// </summary>
    public static readonly IReadOnlyList<string> WidthSeries =
    [
        SCALE_W480,
        SCALE_W828,
        SCALE_W1280,
        SCALE_W1600,
        SCALE_W1920,
        SCALE_W2560
    ];
}
