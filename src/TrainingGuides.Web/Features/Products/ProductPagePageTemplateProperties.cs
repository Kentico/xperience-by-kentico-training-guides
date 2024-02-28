using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerType;
using TrainingGuides.Web.Features.Shared.OptionsProviders.ColorScheme;

namespace TrainingGuides.Web.Features.Products;

public class ProductPagePageTemplateProperties : IPageTemplateProperties
{
    [CheckBoxComponent(
        Label = "Use page builder",
        Order = 0)]
    public bool UsePageBuilder { get; set; } = false;

    [DropDownComponent(
        Label = "Color scheme",
        ExplanationText = "Select the color scheme of the template.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 10)]
    public string ColorScheme { get; set; }

    [DropDownComponent(
        Label = "Corner type",
        ExplanationText = "Select the corner type of the template.",
        DataProviderType = typeof(DropdownEnumOptionProvider<CornerTypeOption>),
        Order = 20)]
    public string CornerType { get; set; }

    [DropDownComponent(
        Label = "Column layout",
        ExplanationText = "Select the layout of the editable areas in the template.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColumnLayoutOption>),
        Order = 30)]
    [VisibleIfTrue(nameof(UsePageBuilder))]
    public string ColumnLayout { get; set; }
}

