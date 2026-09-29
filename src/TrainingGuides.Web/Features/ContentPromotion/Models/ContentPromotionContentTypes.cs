namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// The content types the widget's two selectors offer. Retrieval has to name the same types the
/// selector offered - a content item query cannot select content type-specific fields without
/// being limited to specific content types - so both read these arrays rather than repeating
/// the list and drifting apart.
/// </summary>
public static class ContentPromotionContentTypes
{
    /// <summary>
    /// The page types the widget can actually promote.
    /// </summary>
    public static readonly string[] PROMOTABLE_PAGES =
    [
        ArticlePage.CONTENT_TYPE_NAME,
        ProductPage.CONTENT_TYPE_NAME,
        ServicePage.CONTENT_TYPE_NAME
    ];

    /// <summary>
    /// Page types that exist in the selector only so the content tree can be walked. They hold no
    /// promotable content of their own - see <see cref="CONTAINER_PAGES"/> for why they are here.
    /// </summary>
    public static readonly string[] CONTAINER_PAGES =
    [
        EmptyPage.CONTENT_TYPE_NAME,
        StoreSection.CONTENT_TYPE_NAME
    ];

    /// <summary>
    /// Everything the page selector offers: the promotable types plus the containers standing
    /// between them and the channel root.
    /// </summary>
    public static readonly string[] PAGES = [.. PROMOTABLE_PAGES, .. CONTAINER_PAGES];

    public static readonly string[] CONTENT_ITEMS =
    [
        GeneralArticle.CONTENT_TYPE_NAME,
        Interview.CONTENT_TYPE_NAME,
        Service.CONTENT_TYPE_NAME,
        CatFood.CONTENT_TYPE_NAME,
        DogCollar.CONTENT_TYPE_NAME,
        CatFoodVariant.CONTENT_TYPE_NAME,
        DogCollarVariant.CONTENT_TYPE_NAME
    ];
}
