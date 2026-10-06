namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// The content types the widget's two selectors offer. Retrieval has to name the same types the
/// selector offered - a content item query cannot select content type-specific fields without
/// being limited to specific content types. The selector attributes cannot read these arrays, so
/// ContentPromotionContentTypesTests keeps the two in step. Read-only, so no caller can change what
/// another one retrieves.
/// </summary>
public static class ContentPromotionContentTypes
{
    public static readonly IReadOnlyList<string> PAGES =
    [
        ArticlePage.CONTENT_TYPE_NAME,
        ProductPage.CONTENT_TYPE_NAME,
        ServicePage.CONTENT_TYPE_NAME
    ];

    public static readonly IReadOnlyList<string> CONTENT_ITEMS =
    [
        GeneralArticle.CONTENT_TYPE_NAME,
        Service.CONTENT_TYPE_NAME,
        CatFood.CONTENT_TYPE_NAME,
        DogCollar.CONTENT_TYPE_NAME,
        CatFoodVariant.CONTENT_TYPE_NAME,
        DogCollarVariant.CONTENT_TYPE_NAME
    ];
}
