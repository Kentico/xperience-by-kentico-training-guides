namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;

/// <summary>
/// Defines a mapper for converting web page components into email builder widget models.
/// </summary>
public interface IComponentModelMapper<TWidgetModel>
{
    /// <summary>
    /// Maps a web page component to a widget model based on its GUID.
    /// </summary>
    /// <param name="webPageItemGuid">The GUID of the web page component to map.</param>
    /// <param name="languageName">The language name for the email channel.</param>
    /// <returns>A widget model of type <typeparamref name="TWidgetModel"/>.</returns>
    public Task<TWidgetModel> Map(Guid webPageItemGuid, string languageName);
}
