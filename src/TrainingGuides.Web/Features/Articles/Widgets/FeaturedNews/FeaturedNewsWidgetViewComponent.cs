using TrainingGuides.Web.Features.Articles.Widgets.FeaturedNews;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.Shared.Services;
using TrainingGuides.Web.Features.Articles.Services;

[assembly:
    RegisterWidget(FeaturedNewsWidgetViewComponent.IDENTIFIER, typeof(FeaturedNewsWidgetViewComponent), "Featured news",
        typeof(FeaturedNewsWidgetProperties), Description = "Displays the featured news.", IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedNews;

public class FeaturedNewsWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.FeaturedNewsWidget";

    private readonly IContentItemRetrieverService<ArticlePage> articlePageRetrieverService;
    private readonly IArticlePageService articlePageService;

    public FeaturedNewsWidgetViewComponent(
        IContentItemRetrieverService<ArticlePage> articlePageRetrieverService,
        IArticlePageService articlePageService)
    {
        this.articlePageRetrieverService = articlePageRetrieverService;
        this.articlePageService = articlePageService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(FeaturedNewsWidgetProperties properties)
    {
        var guid = properties.Article?.Select(i => i.WebPageGuid).FirstOrDefault();
        var articlePage = guid.HasValue
            ? await articlePageRetrieverService.RetrieveWebPageByGuid(
                guid.Value,
                ArticlePage.CONTENT_TYPE_NAME,
                3)
            : null;

        var model = articlePage != null
            ? new FeaturedNewsWidgetViewModel()
            {
                Article = await articlePageService.GetArticlePageViewModel(articlePage)
            }
            : null;

        return View("~/Features/Articles/Widgets/FeaturedNews/FeaturedNewsWidget.cshtml", model);
    }
}
