namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// The content types the widget's two selectors offer. Retrieval names the same types, because a
/// query only returns content type-specific fields for named types. The selector attributes cannot
/// read these lists, so ContentPromotionContentTypesTests keeps the two in step.
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
