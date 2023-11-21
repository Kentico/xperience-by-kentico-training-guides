
using CMS.Websites;
using TrainingGuides.Web.Services.Content;
using Kentico.Content.Web.Mvc;
using System.Threading.Tasks;


namespace TrainingGuides.Web.Components.PageTemplates;

public class LandingPagePageTemplateService
{
    private readonly IWebPageDataContextRetriever webPageDataContextRetriver;
    private readonly IWebPageQueryResultMapper webPageQueryResultMapper;
    private readonly IContentItemRetrieverService<LandingPage> contentItemRetriever;


    public LandingPagePageTemplateService(IWebPageDataContextRetriever webPageDataContextRetriver, IWebPageQueryResultMapper webPageQueryResultMapper, IContentItemRetrieverService<LandingPage> contentItemRetriever)
    {
        this.webPageDataContextRetriver = webPageDataContextRetriver;
        this.webPageQueryResultMapper = webPageQueryResultMapper;
        this.contentItemRetriever = contentItemRetriever;
    }

    public async Task<LandingPageViewModel> GetTemplateModel()
    {
        var context = webPageDataContextRetriver.Retrieve();

        var landingPage = await contentItemRetriever.RetrieveWebPageById
            (context.WebPage.WebPageItemID,
            LandingPage.CONTENT_TYPE_NAME,
            container => webPageQueryResultMapper.Map<LandingPage>(container));

        return LandingPageViewModel.GetViewModel(landingPage);
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

