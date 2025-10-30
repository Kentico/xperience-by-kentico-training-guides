using CMS.DataEngine;

namespace TrainingGuides.Web.Features.Shared.Services;

public class ContentTypeService : IContentTypeService
{
    /// <summary>
    /// Gets the content type ID for a given content type name.
    /// </summary>
    /// <param name="contentTypeName"></param>
    /// <returns>Content type ID of provided content type, null if not found</returns>
    public int? GetContentTypeId(string contentTypeName) =>
        DataClassInfoProvider
            .GetDataClassInfo(contentTypeName)?
            .ClassID;
}