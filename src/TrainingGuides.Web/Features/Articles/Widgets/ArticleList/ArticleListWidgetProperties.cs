using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetProperties : IWidgetProperties
{
    [WebPageSelectorComponent(Label = "Select the content tree section", Order = 1, MaximumPages = 1, Sortable = true)]
    public IEnumerable<WebPageRelatedItem>? ContentTreeSection { get; set; }

    [NumberInputComponent(Label = "Number of articles to display", Order = 2)]
    public int TopN { get; set; } = 10;

    [TextInputComponent(Label = "CTA text", Order = 4)]
    public string? CtaText { get; set; }

    [DropDownComponent(Label = "Order articles by", Order = 4,
    Options = "newest-first;Newest first\noldest-first;Oldest first")]
    public string OrderBy { get; set; } = "newest-first";
}