using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.Sections;

public class GeneralEmailSectionProperties : IEmailSectionProperties
{
    [DropDownComponent(
        Label = "Column layout",
        ExplanationText = "Select the layout of the editable areas in the template.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColumnLayoutOption>),
        Order = 10)]
    public string ColumnLayout { get; set; } = nameof(ColumnLayoutOption.OneColumn);

    [DropDownComponent(
        Label = "Corner type",
        ExplanationText = "Select the corner type of the template.",
        DataProviderType = typeof(DropdownEnumOptionProvider<CornerStyleOption>),
        Order = 20)]
    public string CornerStyle { get; set; } = nameof(CornerStyleOption.Round);

    [DropDownComponent(
        Label = "Column 1 color scheme",
        ExplanationText = "Select the color scheme for the first column.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 30)]
    public string Column1ColorScheme { get; set; } = nameof(ColorSchemeOption.Light1);

    [DropDownComponent(
        Label = "Column 2 color scheme",
        ExplanationText = "Select the color scheme for the second column.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 40)]
    [VisibleIfNotEqualTo(nameof(ColumnLayout), nameof(ColumnLayoutOption.OneColumn))]
    public string Column2ColorScheme { get; set; } = nameof(ColorSchemeOption.Light1);

    [DropDownComponent(
        Label = "Column 3 color scheme",
        ExplanationText = "Select the color scheme for the third column.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 50)]
    [FormComponentConfiguration(typeof(GeneralEmailSectionColumn3Configurator), [nameof(ColumnLayout)])]
    public string Column3ColorScheme { get; set; } = nameof(ColorSchemeOption.Light1);

}