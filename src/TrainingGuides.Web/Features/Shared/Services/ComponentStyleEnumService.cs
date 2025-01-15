using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
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
        ColorSchemeOption.TransparentMedium => ["tg-bg-none", "tg-txt-medium"],
        ColorSchemeOption.TransparentDark => ["tg-bg-none", "tg-txt-dark"],
        _ => [string.Empty],
    };

    public IEnumerable<string> GetCornerStyleClasses(CornerStyleOption cornerStyle) => cornerStyle switch
    {
        CornerStyleOption.Round => ["tg-corner-rnd"],
        CornerStyleOption.VeryRound => ["tg-corner-v-rnd"],
        CornerStyleOption.Sharp => ["tg-corner-shrp"],
        _ => [string.Empty],
    };

    public CornerStyleOption GetCornerStyle(string cornerStyleString) =>
        new DropdownEnumOptionProvider<CornerStyleOption>().Parse(cornerStyleString, CornerStyleOption.Round);

    public ColorSchemeOption GetColorScheme(string colorSchemeString) =>
        new DropdownEnumOptionProvider<ColorSchemeOption>().Parse(colorSchemeString, ColorSchemeOption.TransparentDark);

    public ColorSchemeOption GetLinkStyle(string linkStyleString)
    {
        var colorScheme = new DropdownEnumOptionProvider<LinkStyleOption>().Parse(linkStyleString, LinkStyleOption.TransparentDark);

        return (ColorSchemeOption)colorScheme;
    }
}
