using CMS.EmailMarketing;
using Kentico.Xperience.Mjml.StarterKit.Rcl;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Contracts;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;
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

    private IEmailData emailData = new EmailData(string.Empty, string.Empty);
    private CornerStyleOption CornerStyle => EnumStringService.Parse(Properties.CornerStyle, CornerStyleOption.Round);
    private ColorSchemeOption MainColorScheme => EnumStringService.Parse(Properties.MainColorScheme, ColorSchemeOption.Light2);
    private List<string> RowIdentifiers { get; set; } = [];
    private List<string> MainCssClasses { get; set; } = [];

    private string? cssContent;
    protected string CssContent
    {
        get => cssContent ?? string.Empty;
        set => cssContent = value;
    }

    private EmailRecipientContext? recipient;

    protected EmailRecipientContext Recipient => recipient ??= RecipientContextAccessor.GetContext();

    [Parameter]
    public GeneralEmailTemplateProperties Properties { get; set; } = new();

    [Inject]
    private IEmailRecipientContextAccessor RecipientContextAccessor { get; set; } = default!;

    [Inject]
    private IEmailDataMapper EmailDataMapper { get; set; } = default!;

    [Inject]
    private CssLoaderService CssLoaderService { get; set; } = default!;

    [Inject]
    private IComponentStyleEnumService ComponentStyleEnumService { get; set; } = default!;

    [Inject]
    private IEnumStringService EnumStringService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        emailData = await EmailDataMapper.Map();
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