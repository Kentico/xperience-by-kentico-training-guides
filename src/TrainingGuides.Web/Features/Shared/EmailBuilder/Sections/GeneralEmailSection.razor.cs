using Kentico.EmailBuilder.Web.Mvc;
using TrainingGuides.Web.Features.Shared.EmailBuilder.Sections;

using Microsoft.AspNetCore.Components;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders;

[assembly: RegisterEmailSection(
    identifier: GeneralEmailSection.IDENTIFIER,
    name: "General email template section",
    componentType: typeof(GeneralEmailSection),
    PropertiesType = typeof(GeneralEmailSectionProperties),
    Description = "Section for the General email template.",
    IconClass = "icon-l-cols-3")]

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.Sections;



public partial class GeneralEmailSection : ComponentBase
{
    public const string IDENTIFIER = $"TrainingGuides.GeneralEmailSection";

    private ColumnLayoutOption ColumnLayout => new DropdownEnumOptionProvider<ColumnLayoutOption>().Parse(Properties.ColumnLayout, ColumnLayoutOption.OneColumn);
    private CornerStyleOption CornerStyle => new DropdownEnumOptionProvider<CornerStyleOption>().Parse(Properties.CornerStyle, CornerStyleOption.Round);
    private ColorSchemeOption Column1ColorScheme => new DropdownEnumOptionProvider<ColorSchemeOption>().Parse(Properties.Column1ColorScheme, ColorSchemeOption.Light1);
    private ColorSchemeOption Column2ColorScheme => new DropdownEnumOptionProvider<ColorSchemeOption>().Parse(Properties.Column2ColorScheme, ColorSchemeOption.Light1);
    private ColorSchemeOption Column3ColorScheme => new DropdownEnumOptionProvider<ColorSchemeOption>().Parse(Properties.Column3ColorScheme, ColorSchemeOption.Light1);

    [Parameter]
    public GeneralEmailSectionProperties Properties { get; set; } = new();
}
