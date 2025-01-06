using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.OrderBy;

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetProperties : IWidgetProperties
{
    [WebPageSelectorComponent(
        Label = "Select the content tree section",
        MaximumPages = 1,
        Sortable = true,
        Order = 10)]
    public IEnumerable<WebPageRelatedItem> ContentTreeSection { get; set; } = Enumerable.Empty<WebPageRelatedItem>();

    [TagSelectorComponent(
        "ArticleCategory",
        Label = "Filter to categories",
        ExplanationText = "Select 0, 1 or more Article Type tags. Shows all if none are selected",
        Order = 15)]
    public IEnumerable<TagReference> Tags { get; set; } = Enumerable.Empty<TagReference>();

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
        DataProviderType = typeof(DropdownEnumOptionProvider<OrderByOption>),
        Order = 40)]
    public string OrderBy { get; set; } = OrderByOption.NewestFirst.ToString();
}