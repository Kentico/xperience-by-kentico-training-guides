using CMS.Core;
using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Contracts;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.Mappers;

public class TrainingGuidesEmailDataMapper : IEmailDataMapper
{
    private const string DEFAULT_SUBJECT = "Training Guides";
    private const string DEFAULT_PREVIEW_TEXT = "The latest communication from Training Guides.";
    private readonly IEmailContextAccessor emailContextAccessor;
    private readonly IEventLogService eventLogService;

    public TrainingGuidesEmailDataMapper(IEmailContextAccessor emailContextAccessor, IEventLogService eventLogService)
    {
        this.emailContextAccessor = emailContextAccessor;
        this.eventLogService = eventLogService;
    }

    public async Task<IEmailData> Map()
    {
        var emailContext = emailContextAccessor.GetContext();

        try
        {
            var email = await emailContext.GetEmail<BasicEmail>();
            return GetEmailData(email?.EmailSubject, email?.EmailPreviewText);
        }
        catch (InvalidCastException)
        {
            try
            {
                var email = await emailContext.GetEmail<NatureSpotlightEmail>();
                return GetEmailData(email?.EmailSubject, email?.EmailPreviewText);
            }
            catch (InvalidCastException ex)
            {
                eventLogService.LogException("EmailDataMapper", "MAPEMAIL", ex);
                return GetEmailData();
            }
        }
        catch (Exception ex)
        {
            eventLogService.LogException("EmailDataMapper", "MAPEMAIL", ex);
            return GetEmailData();
        }


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