namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;

/// <summary>
/// Defines a mapper for converting content items into email builder widget models.
/// </summary>
public interface IComponentModelMapper<TWidgetModel>
{
    /// <summary>
    /// Maps a content item to a widget model based on its GUID.
    /// </summary>
    /// <param name="itemGuid">The GUID of the content item to map.</param>
    /// <param name="languageName">The language name for the email channel.</param>
    /// <returns>A widget model of type <typeparamref name="TWidgetModel"/>.</returns>
    public Task<TWidgetModel> Map(Guid itemGuid, string languageName);
}
