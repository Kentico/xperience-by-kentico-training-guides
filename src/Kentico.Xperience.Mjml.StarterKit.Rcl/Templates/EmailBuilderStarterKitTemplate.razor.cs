using CMS.EmailEngine;

using Kentico.EmailBuilder.Web.Mvc;

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
    private string? emailSubject;
    private EmailContext? emailContext;

    /// <summary>
    /// Gets or sets the CSS content for the email template.
    /// </summary>
    protected string CssContent
    {
        get => cssContent ?? string.Empty;
        set => cssContent = value;
    }

    /// <summary>
    /// Gets or sets the email subject.
    /// </summary>
    protected string EmailSubject
    {
        get => emailSubject ?? string.Empty;
        set => emailSubject = value;
    }

    /// <summary>
    /// Gets the current email context.
    /// </summary>
    protected EmailContext EmailContext => emailContext ??= EmailContextAccessor.GetContext();

    [Inject]
    private CssLoaderService CssLoaderService { get; set; } = null!;

    [Inject]
    private IEmailContextAccessor EmailContextAccessor { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        emailSubject = (string)EmailContext.EmailFields[nameof(EmailInfo.EmailSubject)];
        cssContent = await CssLoaderService.GetCssAsync();
    }
}
