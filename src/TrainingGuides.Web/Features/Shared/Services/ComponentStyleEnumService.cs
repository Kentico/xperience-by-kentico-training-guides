using TrainingGuides.Web.Features.Shared.OptionProviders.CornerType;
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

    public IEnumerable<string> GetCornerTypeClasses(CornerTypeOption cornerType) => cornerType switch
    {
        CornerTypeOption.Round => ["tg-corner-rnd"],
        CornerTypeOption.VeryRound => ["tg-corner-v-rnd"],
        CornerTypeOption.Sharp => ["tg-corner-shrp"],
        _ => [string.Empty],
    };

    public CornerTypeOption GetCornerType(string cornerTypeString)
    {
        if (!Enum.TryParse(cornerTypeString, out CornerTypeOption cornerType))
        {
            cornerType = CornerTypeOption.Round;
        }

        return cornerType;
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
