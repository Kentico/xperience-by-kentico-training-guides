namespace TrainingGuides.Web.Features.LandingPages;

public class LandingPageViewModel
{
    public string Message { get; set; } = string.Empty;

    public static LandingPageViewModel GetViewModel(LandingPage? landingPage) =>
        landingPage == null
            ? new LandingPageViewModel()
            : new LandingPageViewModel
            {
                Message = landingPage.LandingPageContent.FirstOrDefault()?.LandingContentMessage ?? string.Empty
            };
}

