namespace TrainingGuides.Web.Features.Newsletters.NatureSpotlight;

public interface INatureSpotlightEmailService
{
    /// <summary>
    /// Gets the Nature Spotlight email from the current email context.
    /// </summary>
    Task<NatureSpotlightEmail> GetNatureSpotlightEmailFromContext();

    /// <summary>
    /// Gets the model for the Nature Spotlight email.
    /// </summary>
    Task<NatureSpotlightEmailModel> GetNatureSpotlightEmailModel();
}
