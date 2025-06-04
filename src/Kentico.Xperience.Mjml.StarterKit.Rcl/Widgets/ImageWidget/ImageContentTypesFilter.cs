using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Helpers;

using Microsoft.Extensions.Options;

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

/// <summary>
/// Image content types filter.
/// </summary>
internal sealed class ImageContentTypesFilter : IContentTypesFilter
{
    /// <summary>
    /// Content type GUID identifiers allowed for <see cref="ImageWidget"/>.
    /// </summary>
    public IEnumerable<Guid> AllowedContentTypeIdentifiers { get; }

    /// <summary>
    /// Image content types filter.
    /// </summary>
    /// <param name="mjmlStarterKitOptions">The MJML starter kit options.</param>
    public ImageContentTypesFilter(IOptions<MjmlStarterKitOptions> mjmlStarterKitOptions)
    {
        var codeNames = mjmlStarterKitOptions.Value.AllowedImageContentTypes;

        AllowedContentTypeIdentifiers = DataClassInfoProviderHelper.GetClassGuidsByCodeNames(codeNames);
    }
}
