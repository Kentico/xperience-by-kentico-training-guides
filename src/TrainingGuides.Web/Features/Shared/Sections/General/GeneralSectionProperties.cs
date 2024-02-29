using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.OptionsProviders.ColorScheme;

namespace TrainingGuides.Web.Features.Shared.Sections.General;
public class GeneralSectionProperties : ISectionProperties
{
    [TextInputComponent(
        Label = "Section anchor",
        Order = 0)]
    public string? SectionAnchor { get; set; }

    [DropDownComponent(
        Label = "Color scheme",
        ExplanationText = "Select the color scheme of the section.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 10)]
    public string? ColorScheme { get; set; }

    [DropDownComponent(
        Label = "Corner type",
        ExplanationText = "Select the corner type of the section.",
        DataProviderType = typeof(DropdownEnumOptionProvider<CornerStyleOption>),
        Order = 20)]
    public string? CornerStyle { get; set; }

    [DropDownComponent(
        Label = "Column layout",
        ExplanationText = "Select the layout of the widget zones in the section.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColumnLayoutOption>),
        Order = 30)]
    public string? ColumnLayout { get; set; }
}
