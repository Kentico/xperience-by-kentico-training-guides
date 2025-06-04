using CMS.DataEngine;
using Kentico.Xperience.Admin.Base.FormAnnotations;

using Microsoft.Extensions.Options;
using TrainingGuides.Web.Features.Shared.EmailBuilder;

namespace TrainingGuides.Web.Features.Articles.EmailWidgets;

/// <summary>
/// Article content types filter.
/// </summary>
internal sealed class ArticleContentTypesFilter : IContentTypesFilter
{
    /// <summary>
    /// Content type GUID identifiers allowed for <see cref="ArticleWidget"/>.
    /// </summary>
    public IEnumerable<Guid> AllowedContentTypeIdentifiers { get; }

    /// <summary>
    /// Article content types filter.
    /// </summary>
    /// <param name="trainingGuidesEmailBuilderOptions">The email builder options, configured at startup.</param>
    public ArticleContentTypesFilter(IOptions<TrainingGuidesEmailBuilderOptions> trainingGuidesEmailBuilderOptions)
    {
        var codeNames = trainingGuidesEmailBuilderOptions.Value.AllowedArticleContentTypes;

        AllowedContentTypeIdentifiers = DataClassInfoProvider.ProviderObject.Get()
            .WhereIn(nameof(DataClassInfo.ClassName), codeNames)
            .Column(nameof(DataClassInfo.ClassGUID))
            .GetListResult<Guid>();
    }
}