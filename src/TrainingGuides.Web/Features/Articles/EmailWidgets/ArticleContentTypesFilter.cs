using CMS.DataEngine;
using Kentico.Xperience.Admin.Base.FormAnnotations;

using Microsoft.Extensions.Options;
using TrainingGuides.Web.Features.Shared.EmailBuilder;

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

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
    /// <param name="mjmlStarterKitOptions">The MJML starter kit options.</param>
    public ArticleContentTypesFilter(IOptions<TrainingGuidesEmailBuilderOptions> trainingGuidesEmailBuilderOptions)
    {
        var codeNames = trainingGuidesEmailBuilderOptions.Value.AllowedArticleContentTypes;

        AllowedContentTypeIdentifiers = DataClassInfoProvider.ProviderObject.Get()
            .WhereIn(nameof(DataClassInfo.ClassName), codeNames)
            .Column(nameof(DataClassInfo.ClassGUID))
            .GetListResult<Guid>();
    }
}