using System.ComponentModel;
using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.Heading;

namespace TrainingGuides.Web.Features.FinancialServices.Widgets.ServiceComparator;

// NOTE: For an example of localizing widget properties (labels, explanation texts, and options),
// see CallToActionWidgetProperties in Features/LandingPages/Widgets/CallToAction/

public class ServiceComparatorWidgetProperties : IWidgetProperties
{
    [ContentItemSelectorComponent(
        ServicePage.CONTENT_TYPE_NAME,
        Label = "Selected services",
        MaximumItems = 0,
        Order = 10)]
    public IEnumerable<ContentItemReference> Services { get; set; } = [];

    [TextInputComponent(
        Label = "Comparator heading",
        Order = 20)]
    public string ComparatorHeading { get; set; } = string.Empty;

    [DropDownComponent(
        Label = "Heading type",
        DataProviderType = typeof(DropdownEnumOptionProvider<ServiceComparatorHeadingTypeOption>),
        Order = 30)]
    public string HeadingType { get; set; } = nameof(ServiceComparatorHeadingTypeOption.H2);

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

public enum ServiceComparatorHeadingTypeOption
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
