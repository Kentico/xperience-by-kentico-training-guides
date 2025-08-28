namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Contracts;

/// <summary>
/// Contract defining the essential email data properties required by the Email Builder Starter Kit template.
/// This contract is populated by implementations of <see cref="Mapping.IEmailDataMapper"/>.
/// </summary>
public interface IEmailData
{
    /// <summary>
    /// Gets the email subject line.
    /// </summary>
    string? EmailSubject { get; }

    /// <summary>
    /// Gets the email preview text displayed in email clients.
    /// </summary>
    string? EmailPreviewText { get; }
}
