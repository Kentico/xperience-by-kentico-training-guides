namespace TrainingGuides.Web.Features.Articles.Services;

public interface IArticlePageService
{
    /// <summary>
    /// Creates a new instance of <see cref="ArticlePageViewModel"/>, setting the properties using ArticlePage given as a parameter.
    /// </summary>
    /// <param name="articlePage">Corresponding Article page object.</param>
    /// <returns>New instance of ArticlePageViewModel.</returns>
    Task<ArticlePageViewModel> GetArticlePageViewModel(ArticlePage? articlePage);

    /// <summary>
    /// Creates a new instance of <see cref="ArticlePageViewModel"/>, setting the properties using ArticlePage given as a parameter.
    /// </summary>
    /// <param name="articlePage">Corresponding Article page object.</param>
    /// <returns>New instance of ArticlePageViewModel.</returns>
    /// <remarks>
    /// If the articlePage is secured and the current visitor is not authenticated, the view model will prompt them to sign in.
    Task<ArticlePageViewModel> GetArticlePageViewModelWithSecurity(ArticlePage? articlePage, string signInUrl, bool isAuthenticated);

    /// <summary>
    /// Determines whether the reusable article item referenced by the article page is secured.
    /// </summary>
    /// <param name="articlePage">The article page.</param>
    /// <returns>True if the reusable item that the page references is secured.</returns>
    bool IsReusableArticleSecured(ArticlePage articlePage);

    /// <summary>
    /// Retrieves the language name of the article.
    /// </summary>
    /// <param name="articlePage">The article page</param>
    /// <returns>Language name</returns>
    string GetArticleLanguage(ArticlePage articlePage);
}