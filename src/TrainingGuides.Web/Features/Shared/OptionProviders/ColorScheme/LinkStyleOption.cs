using System.ComponentModel;

namespace TrainingGuides.Web.Features.Shared.OptionsProviders.ColorScheme;

public enum LinkStyleOption
{
    [Description("Button, dark")]
    Dark = ColorSchemeOption.Dark1,

    [Description("Button, medium")]
    Medium = ColorSchemeOption.Dark2,

    [Description("Button, light 1")]
    Light1 = ColorSchemeOption.Light1,

    [Description("Button, light 2")]
    Light2 = ColorSchemeOption.Light2,

    [Description("Button, light 3")]
    Light3 = ColorSchemeOption.Light3,

    [Description("Plain link, dark")]
    TransparentDark = ColorSchemeOption.TransparentDark,

    [Description("Plain link, medium")]
    TransparentMedium = ColorSchemeOption.TransparentMedium,

    [Description("Plain link, light")]
    TransparentLight = ColorSchemeOption.TransparentLight
}
