using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.OrderBy;
using System.ComponentModel;

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
        ExplanationText = "Text for the call to action",
        Order = 30)]
    public string CtaText { get; set; } = string.Empty;

    [TextInputComponent(
        Label = "Sign in text",
        ExplanationText = "Text to display when user is not signed in",
        Order = 35)]
    public string SignInText { get; set; } = string.Empty;

    [DropDownComponent(
        Label = "Order articles by",
        DataProviderType = typeof(DropdownEnumOptionProvider<OrderByOption>),
        Order = 40)]
    public string OrderBy { get; set; } = OrderByOption.NewestFirst.ToString();

    [DropDownComponent(
        Label = "Secured items display mode",
        ExplanationText = "How to handle secured items when users that are not signed in",
        DataProviderType = typeof(DropdownEnumOptionProvider<SecuredOption>),
        Order = 50)]
    public string SecuredItems { get; set; } = SecuredOption.IncludeEverything.ToString();
}

public enum SecuredOption
{
    [Description("Include everything")]
    IncludeEverything,
    [Description("Prompt for login")]
    PromptForLogin,
    [Description("Hide secured items")]
    HideSecuredItems
}