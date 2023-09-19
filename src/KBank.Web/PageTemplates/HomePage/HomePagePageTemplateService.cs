using CMS.DocumentEngine.Types.KBank;
using Kentico.Content.Web.Mvc;

namespace KBank.Web.PageTemplates;

public class HomePagePageTemplateService
{
    private readonly IPageDataContextRetriever pageDataContextRetriver;


    public HomePagePageTemplateService(IPageDataContextRetriever pageDataContextRetriver)
    {
        this.pageDataContextRetriver = pageDataContextRetriver;
    }


    public HomePageViewModel GetTemplateModel()
    {
        var page = pageDataContextRetriver.Retrieve<HomePage>().Page;
        return HomePageViewModel.GetViewModel(page);
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

