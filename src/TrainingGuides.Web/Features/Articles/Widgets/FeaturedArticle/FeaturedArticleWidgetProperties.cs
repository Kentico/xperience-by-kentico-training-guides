using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedArticle;

public class FeaturedArticleWidgetProperties : IWidgetProperties
{
    [ContentItemSelectorComponent(
        ArticlePage.CONTENT_TYPE_NAME,
        Label = "Selected article",
        MaximumItems = 1,
        Order = 10)]
    public IEnumerable<ContentItemReference> Article { get; set; } = Enumerable.Empty<ContentItemReference>();
}
