using System.ComponentModel;
using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;

namespace TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductListing;

public class ProductListingWidgetProperties : IWidgetProperties
{
    [ContentItemSelectorComponent(
        [
            ProductPage.CONTENT_TYPE_NAME,
            StoreSection.CONTENT_TYPE_NAME,
            EmptyPage.CONTENT_TYPE_NAME,
            LandingPage.CONTENT_TYPE_NAME,

        ],
        Label = "Select a parent page to pull products from (optional)",
        ExplanationText = "Leave empty to display products from the current page",
        AllowContentItemCreation = false,
        MaximumItems = 1,
        Order = 10)]
    public IEnumerable<ContentItemReference> ParentPageSelection { get; set; } = [];

    [TextInputComponent(
        Label = "CTA text",
        ExplanationText = "Text for the call to action button on each product",
        Order = 20)]
    public string CtaText { get; set; } = "View Product";

    [TextInputComponent(
        Label = "Sign in text",
        ExplanationText = "Text to display when user is not signed in and secured items are set to 'Prompt for login'",
        Order = 30)]
    public string SignInText { get; set; } = "Sign in to view";

    [DropDownComponent(
        Label = "Secured items display mode",
        ExplanationText = "How to handle secured items when users are not signed in",
        DataProviderType = typeof(DropdownEnumOptionProvider<SecuredOption>),
        Order = 40)]
    public string SecuredItemsDisplayMode { get; set; } = SecuredOption.IncludeEverything.ToString();
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
