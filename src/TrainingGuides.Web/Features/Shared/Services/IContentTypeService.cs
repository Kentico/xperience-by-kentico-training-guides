namespace TrainingGuides.Web.Features.Shared.Services;

public interface IContentTypeService
{
    /// <summary>
    /// Gets the content type ID for a given content type name.
    /// </summary>
    /// <param name="contentTypeName"></param>
    /// <returns>Content type ID of provided content type, null if not found</returns>
    int? GetContentTypeId(string contentTypeName);
}
