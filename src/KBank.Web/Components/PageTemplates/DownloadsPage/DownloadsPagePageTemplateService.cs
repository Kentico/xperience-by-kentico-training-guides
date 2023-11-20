using CMS.Websites;
using TrainingGuides.Web.Services.Content;
using Kentico.Content.Web.Mvc;
using System.Threading.Tasks;

namespace TrainingGuides.Web.Components.PageTemplates;

public class DownloadsPagePageTemplateService
{
    private readonly IWebPageDataContextRetriever webPageDataContextRetriver;
    private readonly IWebPageQueryResultMapper webPageQueryResultMapper;
    private readonly IContentItemRetrieverService<DownloadsPage> contentItemRetriever;

    public DownloadsPagePageTemplateService(IWebPageDataContextRetriever webPageDataContextRetriver, IWebPageQueryResultMapper webPageQueryResultMapper, IContentItemRetrieverService<DownloadsPage> contentItemRetriever)
    {
        this.webPageDataContextRetriver = webPageDataContextRetriver;
        this.webPageQueryResultMapper = webPageQueryResultMapper;
        this.contentItemRetriever = contentItemRetriever;
    }


    public async Task<DownloadsPageViewModel> GetTemplateModel()
    {
        var context = webPageDataContextRetriver.Retrieve();

        var downloadsPage = await contentItemRetriever.RetrieveWebPageById(
            context.WebPage.WebPageItemID,
            DownloadsPage.CONTENT_TYPE_NAME,
            container => webPageQueryResultMapper.Map<DownloadsPage>(container),
            2);

        return DownloadsPageViewModel.GetViewModel(downloadsPage);
    }
}

