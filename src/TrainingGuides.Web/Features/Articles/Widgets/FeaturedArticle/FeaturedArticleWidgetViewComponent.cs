using TrainingGuides.Web.Features.Articles.Widgets.FeaturedArticle;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.Shared.Services;
using TrainingGuides.Web.Features.Articles.Services;

[assembly:
    RegisterWidget(FeaturedArticleWidgetViewComponent.IDENTIFIER, typeof(FeaturedArticleWidgetViewComponent), "Featured article",
        typeof(FeaturedArticleWidgetProperties), Description = "Displays a featured article of your choosing.", IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedArticle;

public class FeaturedArticleWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.FeaturedArticleWidget";

    private readonly IContentItemRetrieverService<ArticlePage> articlePageRetrieverService;
    private readonly IArticlePageService articlePageService;

    public FeaturedArticleWidgetViewComponent(
        IContentItemRetrieverService<ArticlePage> articlePageRetrieverService,
        IArticlePageService articlePageService)
    {
        this.articlePageRetrieverService = articlePageRetrieverService;
        this.articlePageService = articlePageService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(FeaturedArticleWidgetProperties properties)
    {
        var guid = properties.Article?.Select(i => i.WebPageGuid).FirstOrDefault();
        var articlePage = guid.HasValue
            ? await articlePageRetrieverService.RetrieveWebPageByGuid(
                guid.Value,
                ArticlePage.CONTENT_TYPE_NAME,
                3)
            : new ArticlePage();

        var model = articlePage != null
            ? new FeaturedArticleWidgetViewModel()
            {
                Article = await articlePageService.GetArticlePageViewModel(articlePage)
            }
            : new FeaturedArticleWidgetViewModel();

        return View("~/Features/Articles/Widgets/FeaturedArticle/FeaturedArticleWidget.cshtml", model);
    }
}
