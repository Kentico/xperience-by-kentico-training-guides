using System.ComponentModel;

namespace TrainingGuides.Web.Features.Shared.OptionsProviders.ColorScheme;

public enum ColorSchemeOption
{
    [Description("Auto")]
    Auto = 0,

    [Description("Light background, dark text")]
    Light1 = 1,

    [Description("Light background, dark text 2")]
    Light2 = 2,

    [Description("Light background, dark text 3")]
    Light3 = 3,

    [Description("Transparent background, dark text")]
    TransparentDark = 4,

    [Description("Transparent background, light text")]
    TransparentLight = 5,

    [Description("Dark background, light text")]
    Dark1 = 6,

    [Description("Dark background, light text 2")]
    Dark2 = 7,
}
