using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.Templates;

public class GeneralEmailTemplateProperties : IEmailTemplateProperties
{
    [NumberInputComponent(
        Label = "Number of rows",
        ExplanationText = "The number of rows in the template. (maximum of 5)",
        Order = 10)]
    public int NumberOfRows { get; set; } = 1;

    [DropDownComponent(
        Label = "Corner type",
        ExplanationText = "Select the corner type of the template.",
        DataProviderType = typeof(DropdownEnumOptionProvider<CornerStyleOption>),
        Order = 20)]
    public string CornerStyle { get; set; } = nameof(CornerStyleOption.Round);

    [DropDownComponent(
        Label = "Color scheme",
        ExplanationText = "Select the color scheme of the template.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 30)]
    public string MainColorScheme { get; set; } = nameof(ColorSchemeOption.Light2);
}