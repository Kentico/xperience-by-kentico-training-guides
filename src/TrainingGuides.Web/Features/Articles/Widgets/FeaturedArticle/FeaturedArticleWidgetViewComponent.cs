using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Articles.Widgets.FeaturedArticle;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(FeaturedArticleWidgetViewComponent.IDENTIFIER, typeof(FeaturedArticleWidgetViewComponent), "Featured article",
        typeof(FeaturedArticleWidgetProperties), Description = "Displays a featured article of your choosing.", IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedArticle;

public class FeaturedArticleWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.FeaturedArticleWidget";

    private readonly IContentItemRetrieverService contentItemRetrieverService;
    private readonly IArticlePageService articlePageService;

    public FeaturedArticleWidgetViewComponent(
        IContentItemRetrieverService contentItemRetrieverService,
        IArticlePageService articlePageService)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.articlePageService = articlePageService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(FeaturedArticleWidgetProperties properties)
    {
        var guid = properties.Article?.Select(i => i.Identifier).FirstOrDefault();
        var articlePage = guid.HasValue && guid.Value != Guid.Empty
            ? await contentItemRetrieverService.RetrieveWebPageByContentItemGuid<ArticlePage>(
                guid.Value,
                3)
            : null;

        var model = articlePage != null
            ? new FeaturedArticleWidgetViewModel()
            {
                Article = await articlePageService.GetArticlePageViewModel(articlePage)
            }
            : null;

        return View("~/Features/Articles/Widgets/FeaturedArticle/FeaturedArticleWidget.cshtml", model);
    }
}
