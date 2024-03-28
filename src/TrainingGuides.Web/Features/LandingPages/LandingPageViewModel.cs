namespace TrainingGuides.Web.Features.LandingPages;

public class LandingPageViewModel
{
    public string? Message { get; set; }

    public static LandingPageViewModel GetViewModel(LandingPage landingPage)
    {
        if (landingPage == null)
        {
            return new LandingPageViewModel();
        }
        return new LandingPageViewModel
        {
            Message = landingPage.LandingPageContent.FirstOrDefault()?.LandingContentMessage
        };
    }
}