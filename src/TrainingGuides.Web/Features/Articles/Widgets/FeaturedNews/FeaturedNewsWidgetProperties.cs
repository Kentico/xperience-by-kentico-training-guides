using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Websites.FormAnnotations;

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedNews;

public class FeaturedNewsWidgetProperties : IWidgetProperties
{
    [WebPageSelectorComponent(Label = "Selected article", Order = 1, MaximumPages = 1, Sortable = true)]
    public IEnumerable<WebPageRelatedItem> Article { get; set; } = null!;
}
