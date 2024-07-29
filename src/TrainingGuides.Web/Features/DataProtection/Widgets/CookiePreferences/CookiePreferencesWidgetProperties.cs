using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.DataProtection.Widgets.CookiePreferences;

public class CookiePreferencesWidgetProperties : IWidgetProperties
{
    /// <summary>
    /// Essential cookie header.
    /// </summary>
    [TextInputComponent(
        Label = "Essential cookie header",
        Order = 10)]
    public string EssentialHeader { get; set; }

    /// <summary>
    /// Essential cookie description.
    /// </summary>
    [TextInputComponent(
        Label = "Essential cookie description",
        Order = 20)]
    public string EssentialDescription { get; set; }

    /// <summary>
    /// Button text.
    /// </summary>
    [TextInputComponent(
        Label = "Button text",
        Order = 30)]
    public string ButtonText { get; set; }
}
