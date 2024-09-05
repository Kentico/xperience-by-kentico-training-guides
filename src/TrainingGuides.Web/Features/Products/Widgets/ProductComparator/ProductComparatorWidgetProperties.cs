using System.ComponentModel;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.Heading;

namespace TrainingGuides.Web.Features.Products.Widgets.ProductComparator;
public class ProductComparatorWidgetProperties : IWidgetProperties
{
    [WebPageSelectorComponent(
        Label = "Selected products",
        MaximumPages = 0,
        Sortable = true,
        ItemModifierType = typeof(ProductComparatorWidgetItemModifier),
        Order = 10)]
    public IEnumerable<WebPageRelatedItem> Products { get; set; } = Enumerable.Empty<WebPageRelatedItem>();

    [TextInputComponent(
        Label = "Comparator heading",
        Order = 20)]
    public string ComparatorHeading { get; set; } = string.Empty;

    [DropDownComponent(
        Label = "Heading type",
        DataProviderType = typeof(DropdownEnumOptionProvider<ProductComparatorHeadingTypeOption>),
        Order = 30)]
    public string HeadingType { get; set; } = nameof(ProductComparatorHeadingTypeOption.H2);

    [DropDownComponent(
        Label = "Heading margin",
        DataProviderType = typeof(DropdownEnumOptionProvider<HeadingMarginOption>),
        Order = 40)]
    public string HeadingMargin { get; set; } = HeadingMarginOption.Default.ToString();

    [TextInputComponent(
        Label = "CTA",
        Order = 50)]
    public string CallToAction { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "Show short description",
        Order = 60)]
    public bool ShowShortDescription { get; set; }

    [CheckBoxComponent(
        Label = "Show price",
        Order = 70)]
    public bool ShowPrice { get; set; }
}

public enum ProductComparatorHeadingTypeOption
{
    [Description("Heading 2")]
    H2 = HeadingTypeOption.H2,
    [Description("Heading 3")]
    H3 = HeadingTypeOption.H3,
    [Description("Heading 4")]
    H4 = HeadingTypeOption.H4
}

public enum HeadingMarginOption
{
    [Description("Default")]
    Default,
    [Description("Small")]
    Small,
    [Description("Large")]
    Large
}
