using System.ComponentModel;

namespace TrainingGuides.Web.Features.Shared.OptionsProviders.ColorScheme;

public enum ColorSchemeOption
{
    [Description("Auto")]
    Auto = 0,

    [Description("Transparent with light text")]
    TransparentLight = 1,

    [Description("Transparent with dark text")]
    TransparentDark = 2,

    [Description("Dark 1")]
    Dark1 = 3,

    [Description("Dark2")]
    Dark2 = 4,

    [Description("Light 1")]
    Light1 = 5,

    [Description("Light 2")]
    Light2 = 6,

    [Description("Light 3")]
    Light3 = 7,
}
