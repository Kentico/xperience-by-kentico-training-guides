using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl;
using Microsoft.AspNetCore.Components;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.Templates;

public partial class GeneralEmailTemplate : ComponentBase
{
    public const string IDENTIFIER = "TrainingGuides.GeneralEmailTemplate";

    private string Subject { get; set; } = string.Empty;
    private string EmailPreviewText { get; set; } = string.Empty;
    private ColumnLayoutOption ColumnLayout => new DropdownEnumOptionProvider<ColumnLayoutOption>().Parse(Properties.ColumnLayout, ColumnLayoutOption.OneColumn);
    private CornerStyleOption CornerStyle => new DropdownEnumOptionProvider<CornerStyleOption>().Parse(Properties.CornerStyle, CornerStyleOption.Round);
    private ColorSchemeOption MainColorScheme => new DropdownEnumOptionProvider<ColorSchemeOption>().Parse(Properties.MainColorScheme, ColorSchemeOption.Light1);
    private ColorSchemeOption HeaderColorScheme => new DropdownEnumOptionProvider<ColorSchemeOption>().Parse(Properties.HeaderColorScheme, ColorSchemeOption.Light1);
    private ColorSchemeOption Column1ColorScheme => new DropdownEnumOptionProvider<ColorSchemeOption>().Parse(Properties.Column1ColorScheme, ColorSchemeOption.Light1);
    private ColorSchemeOption Column2ColorScheme => new DropdownEnumOptionProvider<ColorSchemeOption>().Parse(Properties.Column2ColorScheme, ColorSchemeOption.Light1);
    private ColorSchemeOption Column3ColorScheme => new DropdownEnumOptionProvider<ColorSchemeOption>().Parse(Properties.Column3ColorScheme, ColorSchemeOption.Light1);
    private ColorSchemeOption FooterColorScheme => new DropdownEnumOptionProvider<ColorSchemeOption>().Parse(Properties.FooterColorScheme, ColorSchemeOption.Light1);
    private List<string> MainCssClasses { get; set; } = [];
    private List<string> HeaderCssClasses { get; set; } = [];
    private List<string> FooterCssClasses { get; set; } = [];
    private string? cssContent;
    protected string CssContent
    {
        get => cssContent ?? string.Empty;
        set => cssContent = value;
    }

    [Parameter]
    public GeneralEmailTemplateProperties Properties { get; set; } = new();

    [Inject]
    private IEmailContextAccessor EmailContextAccessor { get; set; } = null!;

    [Inject]
    private CssLoaderService CssLoaderService { get; set; } = null!;

    [Inject]
    private IComponentStyleEnumService ComponentStyleEnumService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Subject = EmailContextAccessor.GetContext().EmailFields.GetValue("Subject") as string ?? string.Empty;
        EmailPreviewText = EmailContextAccessor.GetContext().EmailFields.GetValue("EmailPreviewText") as string ?? string.Empty;
        CssContent = await CssLoaderService.GetCssAsync();

        var cornerCssClasses = ComponentStyleEnumService.GetCornerStyleClasses(CornerStyle);

        MainCssClasses.AddRange(ComponentStyleEnumService.GetColorSchemeClasses(MainColorScheme));
        MainCssClasses.AddRange(cornerCssClasses);

        HeaderCssClasses.AddRange(ComponentStyleEnumService.GetColorSchemeClasses(HeaderColorScheme));
        HeaderCssClasses.AddRange(cornerCssClasses);

        FooterCssClasses.AddRange(ComponentStyleEnumService.GetColorSchemeClasses(FooterColorScheme));
        FooterCssClasses.AddRange(cornerCssClasses);
    }
}