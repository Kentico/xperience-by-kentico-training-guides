using CMS.DocumentEngine.Types.KBank;
using Kentico.Content.Web.Mvc;

namespace KBank.Web.Components.PageTemplates;

public class DownloadPagePageTemplateService
{
    private readonly IPageDataContextRetriever pageDataContextRetriver;


    public DownloadPagePageTemplateService(IPageDataContextRetriever pageDataContextRetriver)
    {
        this.pageDataContextRetriver = pageDataContextRetriver;
    }


    public DownloadPageViewModel GetTemplateModel()
    {
        var page = pageDataContextRetriver.Retrieve<DownloadPage>().Page;
        return DownloadPageViewModel.GetViewModel(page);
    }
}

