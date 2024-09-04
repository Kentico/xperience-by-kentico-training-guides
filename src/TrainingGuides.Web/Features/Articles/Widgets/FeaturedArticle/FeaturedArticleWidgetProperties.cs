using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Websites.FormAnnotations;

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedArticle;

public class FeaturedArticleWidgetProperties : IWidgetProperties
{
    [WebPageSelectorComponent(
        Label = "Selected article",
        MaximumPages = 1,
        Sortable = true,
        Order = 10)]
    public IEnumerable<WebPageRelatedItem> Article { get; set; } = Enumerable.Empty<WebPageRelatedItem>();
}
