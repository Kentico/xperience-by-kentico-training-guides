using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Contracts;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;
using Microsoft.AspNetCore.Components;

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Templates;

/// <summary>
/// The email starter kit template component.
/// </summary>
public partial class EmailBuilderStarterKitTemplate : ComponentBase
{
    /// <summary>
    /// The component identifier.
    /// </summary>
    public const string IDENTIFIER = $"Kentico.Xperience.Mjml.StarterKit.{nameof(EmailBuilderStarterKitTemplate)}";

    private string? cssContent;
    private EmailContext? emailContext;
    private IEmailData? emailData;

    /// <summary>
    /// Gets or sets the CSS content for the email template.
    /// </summary>
    protected string CssContent
    {
        get => cssContent ?? string.Empty;
        set => cssContent = value;
    }

    /// <summary>
    /// Gets the email subject from the mapped email data.
    /// </summary>
    protected string EmailSubject => emailData?.EmailSubject ?? string.Empty;

    /// <summary>
    /// Gets the email preview text from the mapped email data.
    /// </summary>
    protected string? EmailPreviewText => emailData?.EmailPreviewText;

    /// <summary>
    /// Gets the current email context.
    /// </summary>
    protected EmailContext EmailContext => emailContext ??= EmailContextAccessor.GetContext();

    [Inject]
    private CssLoaderService CssLoaderService { get; set; } = null!;

    [Inject]
    private IEmailContextAccessor EmailContextAccessor { get; set; } = null!;

    [Inject]
    private IEmailDataMapper EmailDataMapper { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        emailData = await EmailDataMapper.Map();

        cssContent = await CssLoaderService.GetCssAsync();
    }
}
