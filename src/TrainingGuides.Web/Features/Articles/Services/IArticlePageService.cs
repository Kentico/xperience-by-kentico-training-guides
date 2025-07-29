namespace TrainingGuides.Web.Features.Articles.Services;

public interface IArticlePageService
{
    ArticlePageViewModel GetArticlePageViewModel(ArticlePage? articlePage);
}