namespace TrainingGuides.Web.Features.Articles.Services;

public interface IArticlePageService
{
    public Task<ArticlePageViewModel> GetArticlePageViewModel(ArticlePage articlePage);
}