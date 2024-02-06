using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionsProviders.Heading;

namespace TrainingGuides.Web.Features.Products.Widgets.ProductComparator;

public class ProductComparatorWidgetProperties : IWidgetProperties
{
    [WebPageSelectorComponent(Label = "Selected products", Order = 1, MaximumPages = 0, Sortable = true, ItemModifierType = typeof(ProductComparatorWidgetItemModifier))]
    public IEnumerable<WebPageRelatedItem> Products { get; set; } = null!;

    [TextInputComponent(Label = "Comparator heading", Order = 2)]
    public string ComparatorHeading { get; set; } = null!;

    [DropDownComponent(DataProviderType = typeof(HeadingTypeOptionsProvider), Label = "Heading type", Order = 3)]
    public string HeadingType { get; set; } = null!;

    [DropDownComponent(Label = "Heading margin", Order = 4,
        Options = "default;Default\nsmall;Small\nlarge;Large")]
    public string HeadingMargin { get; set; } = null!;

    [TextInputComponent(Label = "CTA", Order = 5)]
    public string CallToAction { get; set; } = null!;

    [CheckBoxComponent(Label = "Show short description", Order = 6)]
    public bool ShowShortDescription { get; set; }
}

public class HeadingTypeOptionsProvider : Shared.OptionsProviders.Heading.HeadingTypeOptionsProvider
{
    public HeadingTypeOptionsProvider() : base(new[] { HeadingTypeOption.Auto, HeadingTypeOption.h2, HeadingTypeOption.h3, HeadingTypeOption.h4 })
    {
    }
}
