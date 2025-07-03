namespace TrainingGuides.Web.Features.Newsletters.NatureSpotlight;

public interface INatureSpotlightEmailService
{
    Task<NatureSpotlightEmailModel> GetNatureSpotlightEmailModel();
}
