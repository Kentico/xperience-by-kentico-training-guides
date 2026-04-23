using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.Shared.OptionProviders.OrderBy;
using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Articles.Widgets.ArticleList;
using TrainingGuides.Web.Features.Shared.Services;

// NOTE: For an example of localizing widget name and description,
// see CallToActionWidgetViewComponent in Features/LandingPages/Widgets/CallToAction/

[assembly:
    RegisterWidget(ArticleListWidgetViewComponent.IDENTIFIER, typeof(ArticleListWidgetViewComponent), "Article list widget",
        typeof(ArticleListWidgetProperties), Description = "Displays list of articles.", IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetViewComponent(
    IContentItemRetrieverService contentItemRetrieverService,
    IArticlePageService articlePageService) : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ArticleListWidget";

    public async Task<ViewViewComponentResult> InvokeAsync(ArticleListWidgetProperties properties)
    {
        var model = new ArticleListWidgetViewModel();

        if (properties.ContentTreeSection.Any())
        {
            var articlePages = await RetrieveArticlePages(properties.ContentTreeSection.First(), properties.Tags, properties.SecuredItems);

            model.Articles = (properties.OrderBy.Equals(OrderByOption.OldestFirst.ToString())
                ? (await GetArticlePageViewModels(articlePages, properties.SecuredItems, properties.CtaText, properties.SignInText))
                    .OrderBy(article => article.CreatedOn)
                : (await GetArticlePageViewModels(articlePages, properties.SecuredItems, properties.CtaText, properties.SignInText))
                    .OrderByDescending(article => article.CreatedOn))
                .Take(properties.TopN)
                .ToList();
        }

        return View("~/Features/Articles/Widgets/ArticleList/ArticleListWidget.cshtml", model);
    }

    private async Task<IEnumerable<ArticlePage>> RetrieveArticlePages(ContentItemReference parentPageSelection, IEnumerable<TagReference> tags, string securedItemsDisplayMode)
    {
        bool includeSecuredItems = securedItemsDisplayMode.Equals(SecuredOption.IncludeEverything.ToString())
            || securedItemsDisplayMode.Equals(SecuredOption.PromptForLogin.ToString());
        var selectedPageGuid = parentPageSelection.Identifier;

        var selectedPage = await contentItemRetrieverService.RetrieveWebPageByContentItemGuid(selectedPageGuid);
        string selectedPagePath = selectedPage?.SystemFields.WebPageItemTreePath ?? string.Empty;

        if (string.IsNullOrEmpty(selectedPagePath))
        {
            return [];
        }

        if (!tags.Any())
        {
            return await contentItemRetrieverService.RetrieveWebPageChildrenByPath<ArticlePage>(
                selectedPagePath,
                3,
                includeSecuredItems);
        }
        else
        {
            var tagGuids = tags.Select(tag => tag.Identifier).ToList();

            var taggedArticleIds = (
                await contentItemRetrieverService.RetrieveContentItemsBySchemaAndTags(
                    IArticleSchema.REUSABLE_FIELD_SCHEMA_NAME,
                    nameof(IArticleSchema.ArticleSchemaCategory),
                    tagGuids)
                ).Select(article => article.SystemFields.ContentItemID);

            return await contentItemRetrieverService.RetrieveWebPageChildrenByPathAndReference<ArticlePage>(
                selectedPagePath,
                nameof(ArticlePage.ArticlePageArticleContent),
                taggedArticleIds,
                includeSecuredItems,
                3);
        }
    }

    private async Task<List<ArticlePageViewModel>> GetArticlePageViewModels(
        IEnumerable<ArticlePage?>? articlePages,
        string securedItemsDisplayMode,
        string defaultCtaText,
        string signInCtaText)
    {
        var models = new List<ArticlePageViewModel>();

        if (articlePages != null)
        {
            foreach (var articlePage in articlePages)
            {
                if (articlePage != null)
                {
                    var model = securedItemsDisplayMode.Equals(SecuredOption.PromptForLogin.ToString())
                        ? await articlePageService.GetArticlePageViewModelWithSecurity(articlePage)
                        : articlePageService.GetArticlePageViewModel(articlePage);

                    if (securedItemsDisplayMode.Equals(SecuredOption.HideSecuredItems.ToString())
                        && model.IsSecured)
                    {
                        continue;
                    }
                    // RequiresSignIn is populated by IArticlePageService based on user auth + item access.
                    model.CTAText = model.RequiresSignIn ? signInCtaText : defaultCtaText;
                    models.Add(model);
                }
            }
        }

        return models;
    }
}
