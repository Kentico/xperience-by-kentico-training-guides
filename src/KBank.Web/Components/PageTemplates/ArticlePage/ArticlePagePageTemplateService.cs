using Kentico.Content.Web.Mvc;

namespace KBank.Web.Components.PageTemplates;

public class ArticlePagePageTemplateService
{
    private readonly IWebPageDataContextRetriever webPageDataContextRetriver;


    public ArticlePagePageTemplateService(IWebPageDataContextRetriever webPageDataContextRetriver)
    {
        this.webPageDataContextRetriver = webPageDataContextRetriver;
    }


    public ArticlePageViewModel GetTemplateModel()
    {
        var context = webPageDataContextRetriver.Retrieve();
        return ArticlePageViewModel.GetViewModel(context?.WebPage as ArticlePage);
    }
}