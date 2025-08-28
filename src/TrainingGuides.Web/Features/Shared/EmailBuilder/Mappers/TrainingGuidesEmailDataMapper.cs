using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Contracts;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.Mappers;

public class TrainingGuidesEmailDataMapper : IEmailDataMapper
{
    private const string DEFAULT_SUBJECT = "Training Guides";
    private const string DEFAULT_PREVIEW_TEXT = "The latest communication from Training Guides.";
    private readonly IEmailContextAccessor emailContextAccessor;

    public TrainingGuidesEmailDataMapper(IEmailContextAccessor emailContextAccessor)
    {
        this.emailContextAccessor = emailContextAccessor;
    }

    public async Task<IEmailData> Map()
    {
        var emailContext = emailContextAccessor.GetContext();

        return emailContext.ContentTypeName switch
        {
            // Newsletter email content type
            BasicEmail.CONTENT_TYPE_NAME => await MapBasicEmail(emailContext),

            // Promotional email content type
            NatureSpotlightEmail.CONTENT_TYPE_NAME => await MapNatureSpotlightEmail(emailContext),

            // Default fallback for unknown content types
            _ => GetEmailData(DEFAULT_SUBJECT, DEFAULT_PREVIEW_TEXT)
        };
    }

    /// <summary>
    /// Maps a BasicEmail content item to an EmailData object, using its subject and preview text.
    /// </summary>
    /// <param name="emailContext">The email context containing the BasicEmail item.</param>
    /// <returns>EmailData with subject and preview text from the BasicEmail item, or default values if null.</returns>
    private async Task<EmailData> MapBasicEmail(EmailContext emailContext)
    {
        var email = await emailContext.GetEmail<BasicEmail>();

        return GetEmailData(email?.EmailSubject, email?.EmailPreviewText);
    }

    /// <summary>
    /// Maps a NatureSpotlightEmail content item to an EmailData object, using its subject and preview text.
    /// </summary>
    /// <param name="emailContext">The email context containing the NatureSpotlightEmail item.</param>
    /// <returns>EmailData with subject and preview text from the NatureSpotlightEmail item, or default values if null.</returns>
    private async Task<EmailData> MapNatureSpotlightEmail(EmailContext emailContext)
    {
        var email = await emailContext.GetEmail<NatureSpotlightEmail>();

        return GetEmailData(email?.EmailSubject, email?.EmailPreviewText);
    }

    /// <summary>
    /// Returns EmailData with the provided subject and preview text values, reverting to default values if they are not passed or null.
    /// </summary>
    /// <param name="subject">The subject of the email</param>
    /// <param name="previewText">The preview text of the email</param>
    /// <returns>EmailData using the provided subject and preview text</returns>
    private EmailData GetEmailData(string? subject = null, string? previewText = null) =>
        new(subject ?? DEFAULT_SUBJECT, previewText ?? DEFAULT_PREVIEW_TEXT);
}