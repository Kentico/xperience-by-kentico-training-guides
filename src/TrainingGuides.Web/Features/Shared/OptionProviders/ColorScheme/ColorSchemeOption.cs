using System.ComponentModel;

namespace TrainingGuides.Web.Features.Shared.OptionsProviders.ColorScheme;

public enum ColorSchemeOption
{
    [Description("Auto")]
    Auto = 0,

    [Description("Transparent background, light text")]
    TransparentLight = 1,

    [Description("Transparent background, dark text")]
    TransparentDark = 2,

    [Description("Dark background, light text")]
    Dark1 = 3,

    [Description("Dark background, light text 2")]
    Dark2 = 4,

    [Description("Light background, dark text")]
    Light1 = 5,

    [Description("Light background, dark text 2")]
    Light2 = 6,

    [Description("Light background, dark text 3")]
    Light3 = 7,
}
