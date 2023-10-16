using System.Linq;

namespace KBank.Web.Components.PageTemplates;

public class LandingPageViewModel
{
    public string Message { get; set; }

    public static LandingPageViewModel GetViewModel(LandingPage landingPage)
    {
        if (landingPage == null)
        {
            return new LandingPageViewModel();
        }
        return new LandingPageViewModel
        {
            Message = landingPage.LandingPageContent.FirstOrDefault()?.Message
        };
    }
}

