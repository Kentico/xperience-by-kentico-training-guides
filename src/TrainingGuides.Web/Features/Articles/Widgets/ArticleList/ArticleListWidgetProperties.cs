using System.ComponentModel;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetProperties : IWidgetProperties
{
    [WebPageSelectorComponent(
        Label = "Select the content tree section",
        Order = 10,
        MaximumPages = 1,
        Sortable = true)]
    public IEnumerable<WebPageRelatedItem>? ContentTreeSection { get; set; }

    [NumberInputComponent(
        Label = "Number of articles to display",
        Order = 20)]
    public int TopN { get; set; } = 10;

    [TextInputComponent(
        Label = "CTA text",
        Order = 30)]
    public string? CtaText { get; set; }

    [DropDownComponent(
        Label = "Order articles by",
        DataProviderType = typeof(DropdownEnumOptionProvider<OrderByOption>),
        Order = 40)]
    public string OrderBy { get; set; } = OrderByOption.NewestFirst.ToString();
}

public enum OrderByOption
{
    [Description("Newest first")]
    NewestFirst,
    [Description("Oldest first")]
    OldestFirst

}