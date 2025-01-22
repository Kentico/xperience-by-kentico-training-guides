namespace TrainingGuides.Web.Features.Shared.Services;

public interface IHttpRequestService
{
    /// <summary>
    /// Retrieves Base URL from the current request context.
    /// </summary>
    /// <returns>The base URL. If current request contains language, it will NOT be returned with the base URL.</returns>
    /// <exception cref="NullReferenceException">Thrown when unable to retrieve current request context.</exception>
    string GetBaseUrl();

    /// <summary>
    /// Combines URL paths
    /// </summary>
    /// <param name="paths">String paths to combine.</param>
    /// <returns>Combined paths</returns>
    /// <remarks>Works with or without leading and trailing slashes</remarks>
    string CombineUrlPaths(params string[] paths);
}