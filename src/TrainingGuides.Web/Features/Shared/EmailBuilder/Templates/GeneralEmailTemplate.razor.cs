using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl;
using Microsoft.AspNetCore.Components;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.Templates;

public partial class GeneralEmailTemplate : ComponentBase
{
    public const string IDENTIFIER = "TrainingGuides.GeneralEmailTemplate";

    public const string AREA_MAIN = "MainContent";

    private string Subject { get; set; } = string.Empty;
    private string EmailPreviewText { get; set; } = string.Empty;
    private CornerStyleOption CornerStyle => new DropdownEnumOptionProvider<CornerStyleOption>().Parse(Properties.CornerStyle, CornerStyleOption.Round);
    private ColorSchemeOption MainColorScheme => new DropdownEnumOptionProvider<ColorSchemeOption>().Parse(Properties.MainColorScheme, ColorSchemeOption.Light2);
    private List<string> RowIdentifiers { get; set; } = [];
    private List<string> MainCssClasses { get; set; } = [];

    private string? cssContent;
    protected string CssContent
    {
        get => cssContent ?? string.Empty;
        set => cssContent = value;
    }

    [Parameter]
    public GeneralEmailTemplateProperties Properties { get; set; } = new();

    [Inject]
    private IEmailContextAccessor EmailContextAccessor { get; set; } = default!;

    [Inject]
    private CssLoaderService CssLoaderService { get; set; } = default!;

    [Inject]
    private IComponentStyleEnumService ComponentStyleEnumService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Subject = EmailContextAccessor.GetContext().EmailFields.GetValue("Subject") as string ?? string.Empty;
        EmailPreviewText = EmailContextAccessor.GetContext().EmailFields.GetValue("EmailPreviewText") as string ?? string.Empty;
        CssContent = await CssLoaderService.GetCssAsync();

        var cornerCssClasses = ComponentStyleEnumService.GetCornerStyleClasses(CornerStyle);

        MainCssClasses.AddRange(ComponentStyleEnumService.GetColorSchemeClasses(MainColorScheme));
        MainCssClasses.AddRange(cornerCssClasses);

        int numberOfRows = Math.Clamp(Properties.NumberOfRows, 1, 5);
        for (int index = 0; index < numberOfRows; index++)
        {
            string rowIdentifier = index == 0 ? AREA_MAIN : $"Area_{index}";
            RowIdentifiers.Add(rowIdentifier);
        }

    }
}