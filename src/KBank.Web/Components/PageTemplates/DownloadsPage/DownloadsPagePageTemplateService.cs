using Kentico.Content.Web.Mvc;

namespace KBank.Web.Components.PageTemplates;

public class DownloadsPagePageTemplateService
{
    private readonly IWebPageDataContextRetriever webPageDataContextRetriver;


    public DownloadsPagePageTemplateService(IWebPageDataContextRetriever webPageDataContextRetriver)
    {
        this.webPageDataContextRetriver = webPageDataContextRetriver;
    }


    public DownloadsPageViewModel GetTemplateModel()
    {
        var context = webPageDataContextRetriver.Retrieve();
        return DownloadsPageViewModel.GetViewModel(context as DownloadsPage);
    }
}

