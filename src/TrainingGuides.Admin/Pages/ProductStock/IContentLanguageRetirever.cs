

using CMS.ContentEngine;

namespace TrainingGuides.Admin.ProductStock;

public interface IDefaultContentLanguageRetriever
{
    /// <summary>
    /// Retrieves the default content language.
    /// </summary>
    /// <returns>The default content language as a ContentLanguageInfo object.</returns>
    Task<ContentLanguageInfo> Get(CancellationToken cancellationToken = default);
}