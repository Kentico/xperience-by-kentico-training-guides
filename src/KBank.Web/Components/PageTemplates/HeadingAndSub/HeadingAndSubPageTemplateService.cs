using Kentico.Content.Web.Mvc;
using CMS.DocumentEngine.Types.KBank;

namespace KBank.Web.Components.PageTemplates;

public class HeadingAndSubPageTemplateService
{
    private readonly IPageDataContextRetriever pageDataContextRetriver;


    public HeadingAndSubPageTemplateService(IPageDataContextRetriever pageDataContextRetriver)
    {
        this.pageDataContextRetriver = pageDataContextRetriver;
    }


    public HeadingAndSubViewModel GetTemplateModel()
    {
        var page = pageDataContextRetriver.Retrieve<HeadingAndSub>().Page;
        return HeadingAndSubViewModel.GetViewModel(page);
    }
}