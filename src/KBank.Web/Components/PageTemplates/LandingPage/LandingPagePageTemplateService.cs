using Kentico.Content.Web.Mvc;

namespace KBank.Web.Components.PageTemplates;

public class LandingPagePageTemplateService
{
    private readonly IWebPageDataContextRetriever webPageDataContextRetriver;


    public LandingPagePageTemplateService(IWebPageDataContextRetriever webPageDataContextRetriver)
    {
        this.webPageDataContextRetriver = webPageDataContextRetriver;
    }


    public LandingPageViewModel GetTemplateModel()
    {
        var context = webPageDataContextRetriver.Retrieve();
        return LandingPageViewModel.GetViewModel(context?.WebPage as LandingPage);
    }

    //ensures only valid strings will be rendered as raw html
    public string GetSafeTagText(string messageType)
    {
        return messageType switch
           {
               "h1" => "h1",
               "h2" => "h2",
               "h3" => "h3",
               "h4" => "h4",
               "p" => "p",
               _ => "span"
           };
    }
}

