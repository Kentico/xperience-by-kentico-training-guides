﻿using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWebPageRoute(
    contentTypeName: LandingPage.CONTENT_TYPE_NAME,
    controllerType: typeof(TrainingGuides.Web.Features.LandingPages.LandingPageController))]

namespace TrainingGuides.Web.Features.LandingPages;
public class LandingPageController : Controller
{
    private readonly IWebPageDataContextRetriever webPageDataContextRetriver;
    private readonly IContentItemRetrieverService<LandingPage> contentItemRetriever;

    public LandingPageController(IWebPageDataContextRetriever webPageDataContextRetriver,
    IContentItemRetrieverService<LandingPage> contentItemRetriever)
    {
        this.webPageDataContextRetriver = webPageDataContextRetriver;
        this.contentItemRetriever = contentItemRetriever;
    }

    public async Task<IActionResult> Index()
    {
        var context = webPageDataContextRetriver.Retrieve();

        var landingPage = await contentItemRetriever.RetrieveWebPageById
            (context.WebPage.WebPageItemID,
            LandingPage.CONTENT_TYPE_NAME);

        var model = LandingPageViewModel.GetViewModel(landingPage);

        return new TemplateResult(model);
    }
}