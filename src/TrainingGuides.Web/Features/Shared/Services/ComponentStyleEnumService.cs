using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.OptionsProviders.ColorScheme;

namespace TrainingGuides.Web.Features.Shared.Services;

public class ComponentStyleEnumService : IComponentStyleEnumService
{
    public IEnumerable<string> GetColorSchemeClasses(ColorSchemeOption colorScheme) => colorScheme switch
    {
        ColorSchemeOption.Light1 => ["tg-bg-light-1", "tg-txt-dark"],
        ColorSchemeOption.Light2 => ["tg-bg-light-2", "tg-txt-dark"],
        ColorSchemeOption.Light3 => ["tg-bg-light-3", "tg-txt-dark"],
        ColorSchemeOption.Dark1 => ["tg-bg-primary", "tg-txt-light"],
        ColorSchemeOption.Dark2 => ["tg-bg-secondary", "tg-txt-light"],
        ColorSchemeOption.TransparentLight => ["tg-bg-none", "tg-txt-light"],
        ColorSchemeOption.TransparentDark or
        ColorSchemeOption.Auto => ["tg-bg-none", "tg-txt-dark"],
        _ => [string.Empty],
    };

    public IEnumerable<string> GetCornerStyleClasses(CornerStyleOption cornerStyle) => cornerStyle switch
    {
        CornerStyleOption.Round => ["tg-corner-rnd"],
        CornerStyleOption.VeryRound => ["tg-corner-v-rnd"],
        CornerStyleOption.Sharp => ["tg-corner-shrp"],
        _ => [string.Empty],
    };

    public CornerStyleOption GetCornerStyle(string cornerStyleString)
    {
        if (!Enum.TryParse(cornerStyleString, out CornerStyleOption cornerStyle))
        {
            cornerStyle = CornerStyleOption.Round;
        }

        return cornerStyle;
    }

    public ColorSchemeOption GetColorScheme(string colorSchemeString)
    {
        if (!Enum.TryParse(colorSchemeString, out ColorSchemeOption colorScheme))
        {
            colorScheme = ColorSchemeOption.TransparentDark;
        }

        return colorScheme;
    }
}
