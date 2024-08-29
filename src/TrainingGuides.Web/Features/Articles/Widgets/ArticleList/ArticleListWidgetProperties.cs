using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetProperties : IWidgetProperties
{
    [WebPageSelectorComponent(
        Label = "Select the content tree section",
        MaximumPages = 1,
        Sortable = true,
        Order = 10)]
    public IEnumerable<WebPageRelatedItem> ContentTreeSection { get; set; } = Enumerable.Empty<WebPageRelatedItem>();

    [NumberInputComponent(
        Label = "Number of articles to display",
        Order = 20)]
    public int TopN { get; set; } = 10;

    [TextInputComponent(
        Label = "CTA text",
        Order = 30)]
    public string CtaText { get; set; } = string.Empty;

    [DropDownComponent(
        Label = "Order articles by",
        Options = "NewestFirst;Newest first\r\nOldestFirst;Oldest first",
        Order = 40)]
    public string OrderBy { get; set; } = "NewstFirst";
}