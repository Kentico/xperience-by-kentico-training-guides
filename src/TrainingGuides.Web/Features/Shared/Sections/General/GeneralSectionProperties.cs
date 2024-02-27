using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.OptionsProviders.ColorScheme;

namespace TrainingGuides.Web.Features.Shared.Sections.General;
public class GeneralSectionProperties : ISectionProperties
{
    [TextInputComponent(Label = "Section anchor", Order = 1)]
    public string? SectionAnchor { get; set; }

    [DropDownComponent(
        Label = "Column layout",
        ExplanationText = "Select the layout of the widget zones in this section.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColumnLayoutOption>),
        Order = 10
    )]
    public string? ColumnLayout { get; set; }

    [DropDownComponent(
        Label = "Color scheme",
        ExplanationText = "Select the color scheme of this section.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 20
    )]
    public string? ColorScheme { get; set; }

    [CheckBoxComponent(
        Label = "Rounded corners",
        ExplanationText = "Check to make the corners of this section rounded",
        Order = 30
    )]
    public bool RondedCorners { get; set; } = false;
}
