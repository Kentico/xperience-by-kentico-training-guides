using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedArticle;

// NOTE: For an example of localizing widget properties (labels, explanation texts, and options),
// see CallToActionWidgetProperties in Features/LandingPages/Widgets/CallToAction/

public class FeaturedArticleWidgetProperties : IWidgetProperties
{
    [ContentItemSelectorComponent(
        ArticlePage.CONTENT_TYPE_NAME,
        Label = "Selected article",
        MaximumItems = 1,
        Order = 10)]
    public IEnumerable<ContentItemReference> Article { get; set; } = [];
}
