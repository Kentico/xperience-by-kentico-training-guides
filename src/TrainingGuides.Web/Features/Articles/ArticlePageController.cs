﻿using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWebPageRoute(
    contentTypeName: ArticlePage.CONTENT_TYPE_NAME,
    controllerType: typeof(TrainingGuides.Web.Features.Articles.ArticlePageController))]

namespace TrainingGuides.Web.Features.Articles;
public class ArticlePageController : Controller
{

    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IContentItemRetrieverService<ArticlePage> contentItemRetriever;

    public ArticlePageController(IWebPageDataContextRetriever webPageDataContextRetriever,
        IContentItemRetrieverService<ArticlePage> contentItemRetriever)
    {
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.contentItemRetriever = contentItemRetriever;
    }

    public async Task<IActionResult> Index()
    {
        var context = webPageDataContextRetriever.Retrieve();
        var articlePage = await contentItemRetriever.RetrieveWebPageById(
            context.WebPage.WebPageItemID,
            ArticlePage.CONTENT_TYPE_NAME,
            2);

        var model = ArticlePageViewModel.GetViewModel(articlePage);
        return new TemplateResult(model);
    }
}