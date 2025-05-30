using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Helpers;

using Microsoft.Extensions.Options;

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

/// <summary>
/// Product content types filter.
/// </summary>
internal sealed class ProductContentTypesFilter : IContentTypesFilter
{
    /// <summary>
    /// Content type GUID identifiers allowed for <see cref="ProductWidget"/>.
    /// </summary>
    public IEnumerable<Guid> AllowedContentTypeIdentifiers { get; }

    /// <summary>
    /// Product content types filter.
    /// </summary>
    /// <param name="mjmlStarterKitOptions">The MJML starter kit options.</param>
    public ProductContentTypesFilter(IOptions<MjmlStarterKitOptions> mjmlStarterKitOptions)
    {
        var codeNames = mjmlStarterKitOptions.Value.AllowedProductContentTypes;

        AllowedContentTypeIdentifiers = DataClassInfoProviderHelper.GetClassGuidsByCodeNames(codeNames);
    }
}
