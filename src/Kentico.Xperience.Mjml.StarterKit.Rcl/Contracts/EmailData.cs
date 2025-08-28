namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Contracts;

/// <summary>
/// Default implementation of the email data contract.
/// </summary>
public class EmailData : IEmailData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailData"/> class.
    /// </summary>
    /// <param name="emailSubject">The email subject line.</param>
    /// <param name="emailPreviewText">The email preview text.</param>
    public EmailData(string? emailSubject, string? emailPreviewText = null)
    {
        EmailSubject = emailSubject ?? string.Empty;
        EmailPreviewText = emailPreviewText;
    }

    /// <inheritdoc />
    public string? EmailSubject { get; }

    /// <inheritdoc />
    public string? EmailPreviewText { get; }
}
