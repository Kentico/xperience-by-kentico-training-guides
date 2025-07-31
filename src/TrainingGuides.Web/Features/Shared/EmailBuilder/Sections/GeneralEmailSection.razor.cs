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
    public const string IDENTIFIER = "TrainingGuides.GeneralEmailSection";

    [Inject]
    private IEnumStringService EnumStringService { get; set; } = default!;

    private ColumnLayoutOption ColumnLayout => EnumStringService.Parse(Properties.ColumnLayout, ColumnLayoutOption.OneColumn);
    private CornerStyleOption CornerStyle => EnumStringService.Parse(Properties.CornerStyle, CornerStyleOption.Round);
    private ColorSchemeOption Column1ColorScheme => EnumStringService.Parse(Properties.Column1ColorScheme, ColorSchemeOption.Light1);
    private ColorSchemeOption Column2ColorScheme => EnumStringService.Parse(Properties.Column2ColorScheme, ColorSchemeOption.Light1);
    private ColorSchemeOption Column3ColorScheme => EnumStringService.Parse(Properties.Column3ColorScheme, ColorSchemeOption.Light1);

    [Parameter]
    public GeneralEmailSectionProperties Properties { get; set; } = new();
}
