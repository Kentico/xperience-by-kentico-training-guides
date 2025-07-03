using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;
using Microsoft.AspNetCore.Components;
using TrainingGuides.Web.Features.Articles.EmailWidgets;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Newsletters.NatureSpotlight;

public class NatureSpotlightEmailService : INatureSpotlightEmailService
{
    private readonly IEmailContextAccessor emailContextAccessor;
    private readonly ICountryService countryService;

    public NatureSpotlightEmailService(
        IEmailContextAccessor emailContextAccessor,
        ICountryService countryService)
    {
        this.emailContextAccessor = emailContextAccessor;
        this.countryService = countryService;
    }

    public async Task<NatureSpotlightEmailModel> GetNatureSpotlightEmailModel()
    {
        var email = await emailContextAccessor.GetContext().GetEmail<NatureSpotlightEmail>();

        var model = new NatureSpotlightEmailModel
        {
            Topic = email.NatureSpotlightTopic,
            Text = new MarkupString(email.NatureSpotlightText),
            Countries = countryService.GetCountryDisplayNamesByGuids(email.NatureSpotlightCountries),
            RelatedArticles = GetNatureSpotlightRelatedArticles(email),
            Images = GetNatureSpotlightImages(email),
        };

        return model;
    }

    private IEnumerable<ArticleEmailWidgetModel> GetNatureSpotlightRelatedArticles(NatureSpotlightEmail email) =>
        email.NatureSpotlightRelatedArticles
            .Select(article => new ArticleEmailWidgetModel
            {
                ArticleTitle = article.ArticlePageArticleContent?.FirstOrDefault()?.ArticleSchemaTitle
                    ?? article.ArticlePageContent?.FirstOrDefault()?.ArticleTitle
                    ?? string.Empty,
                ArticleSummary = article.ArticlePageArticleContent?.FirstOrDefault()?.ArticleSchemaSummary
                    ?? article.ArticlePageContent?.FirstOrDefault()?.ArticleSummary
                    ?? string.Empty,
                ArticleUrl = article.GetUrl().AbsoluteUrl,
                ArticleTeaserImage = new ImageWidgetModel
                {
                    ImageUrl = article.ArticlePageArticleContent?.FirstOrDefault()?.ArticleSchemaTeaser.FirstOrDefault()?.AssetFile.Url
                        ?? article.ArticlePageContent?.FirstOrDefault()?.ArticleTeaser.FirstOrDefault()?.AssetFile.Url
                        ?? string.Empty,
                    AltText = article.ArticlePageArticleContent?.FirstOrDefault()?.ArticleSchemaTeaser.FirstOrDefault()?.AssetAltText
                        ?? article.ArticlePageContent?.FirstOrDefault()?.ArticleTeaser.FirstOrDefault()?.AssetAltText
                        ?? string.Empty
                }
            });

    private IEnumerable<ImageWidgetModel> GetNatureSpotlightImages(NatureSpotlightEmail email) =>
        email.NatureSpotlightImages
            .Select(image => new ImageWidgetModel
            {
                ImageUrl = image.AssetFile.Url,
                AltText = image.AssetAltText
            });
}