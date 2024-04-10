﻿using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWebPageRoute(
    contentTypeName: DownloadsPage.CONTENT_TYPE_NAME,
    controllerType: typeof(TrainingGuides.Web.Features.Downloads.DownloadsPageController))]

namespace TrainingGuides.Web.Features.Downloads;
public class DownloadsPageController : Controller
{
    private readonly IWebPageDataContextRetriever webPageDataContextRetriver;
    private readonly IContentItemRetrieverService<DownloadsPage> contentItemRetriever;

    public DownloadsPageController(IWebPageDataContextRetriever webPageDataContextRetriver,
        IContentItemRetrieverService<DownloadsPage> contentItemRetriever)
    {
        this.webPageDataContextRetriver = webPageDataContextRetriver;
        this.contentItemRetriever = contentItemRetriever;
    }

    public async Task<IActionResult> Index()
    {
        var context = webPageDataContextRetriver.Retrieve();

        var downloadsPage = await contentItemRetriever.RetrieveWebPageById(
            context.WebPage.WebPageItemID,
            DownloadsPage.CONTENT_TYPE_NAME,
            2);

        var model = DownloadsPageViewModel.GetViewModel(downloadsPage);
        return new TemplateResult(model);
    }
}