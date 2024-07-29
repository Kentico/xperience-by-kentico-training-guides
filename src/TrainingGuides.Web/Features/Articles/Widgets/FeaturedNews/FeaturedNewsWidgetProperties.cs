using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Websites.FormAnnotations;

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedNews;

public class FeaturedNewsWidgetProperties : IWidgetProperties
{
    [WebPageSelectorComponent(
        Label = "Selected article",
        MaximumPages = 1,
        Sortable = true,
        Order = 10)]
    public IEnumerable<WebPageRelatedItem> Article { get; set; } = null!;
}
