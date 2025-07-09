using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.Templates;

public class GeneralEmailTemplateProperties : IEmailTemplateProperties
{
    [CheckBoxComponent(
        Label = "Display header area",
        ExplanationText = "When selected, the header will be displayed at the top of the email.",
        Order = 10)]
    public bool DisplayHeader { get; set; } = true;

    [DropDownComponent(
        Label = "Column layout",
        ExplanationText = "Select the layout of the editable areas in the template.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColumnLayoutOption>),
        Order = 20)]
    public string ColumnLayout { get; set; } = nameof(ColumnLayoutOption.OneColumn);

    [CheckBoxComponent(
        Label = "Display footer area",
        ExplanationText = "When selected, the footer will be displayed at the bottom of the email.",
        Order = 30)]
    public bool DisplayFooter { get; set; } = true;

    [DropDownComponent(
        Label = "Corner type",
        ExplanationText = "Select the corner type of the template.",
        DataProviderType = typeof(DropdownEnumOptionProvider<CornerStyleOption>),
        Order = 40)]
    public string CornerStyle { get; set; } = nameof(CornerStyleOption.Round);

    [DropDownComponent(
        Label = "Color scheme",
        ExplanationText = "Select the color scheme of the template.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 50)]
    public string MainColorScheme { get; set; } = nameof(ColorSchemeOption.TransparentDark);

    [DropDownComponent(
        Label = "Header color scheme",
        ExplanationText = "Select the color scheme for the header.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 60)]
    [VisibleIfTrue(nameof(DisplayHeader))]
    public string HeaderColorScheme { get; set; } = nameof(ColorSchemeOption.Light1);

    [DropDownComponent(
        Label = "Column 1 color scheme",
        ExplanationText = "Select the color scheme for the first column.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 70)]
    public string Column1ColorScheme { get; set; } = nameof(ColorSchemeOption.Light1);

    [DropDownComponent(
        Label = "Column 2 color scheme",
        ExplanationText = "Select the color scheme for the second column.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 80)]
    [VisibleIfNotEqualTo(nameof(ColumnLayout), nameof(ColumnLayoutOption.OneColumn))]
    public string Column2ColorScheme { get; set; } = nameof(ColorSchemeOption.Light1);

    [DropDownComponent(
        Label = "Column 3 color scheme",
        ExplanationText = "Select the color scheme for the third column.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 90)]
    [FormComponentConfiguration(typeof(GeneralPageTemplateColumn3Configurator), [nameof(ColumnLayout)])]
    public string Column3ColorScheme { get; set; } = nameof(ColorSchemeOption.Light1);

    [DropDownComponent(
        Label = "Footer color scheme",
        ExplanationText = "Select the color scheme for the footer.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 100)]
    [VisibleIfTrue(nameof(DisplayFooter))]
    public string FooterColorScheme { get; set; } = nameof(ColorSchemeOption.Light1);
}