using CMS.EmailMarketing;
using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl;
using Microsoft.AspNetCore.Components;
using TrainingGuides;
using TrainingGuides.Web.Features.Newsletters.NatureSpotlight;

[assembly: RegisterEmailTemplate(
    identifier: NatureSpotlightEmailTemplate.IDENTIFIER,
    name: "Nature spotlight template",
    componentType: typeof(NatureSpotlightEmailTemplate),
    ContentTypeNames = [NatureSpotlightEmail.CONTENT_TYPE_NAME])]

namespace TrainingGuides.Web.Features.Newsletters.NatureSpotlight;


public partial class NatureSpotlightEmailTemplate
{
    public const string IDENTIFIER = $"TrainingGuides.{nameof(NatureSpotlightEmailTemplate)}";

    private string? cssContent;

    protected string CssContent
    {
        get => cssContent ?? string.Empty;
        set => cssContent = value;
    }

    private EmailRecipientContext? recipient;

    protected EmailRecipientContext Recipient => recipient ??= RecipientContextAccessor.GetContext();

    [Inject]
    private IEmailRecipientContextAccessor RecipientContextAccessor { get; set; } = default!;

    [Inject]
    private INatureSpotlightEmailService NatureSpotlightEmailService { get; set; } = default!;

    [Inject]
    private CssLoaderService CssLoaderService { get; set; } = null!;

    public NatureSpotlightEmailModel Model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        Model = await NatureSpotlightEmailService.GetNatureSpotlightEmailModel();
        cssContent = await CssLoaderService.GetCssAsync();
    }
}