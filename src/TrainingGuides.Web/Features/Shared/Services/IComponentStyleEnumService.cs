using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;

namespace TrainingGuides.Web.Features.Shared.Services;

public interface IComponentStyleEnumService
{
    IEnumerable<string> GetColorSchemeClasses(ColorSchemeOption colorScheme);

    IEnumerable<string> GetCornerStyleClasses(CornerStyleOption cornerStyle);

    CornerStyleOption GetCornerStyle(string cornerStyleString);

    ColorSchemeOption GetColorScheme(string colorSchemeString);

    ColorSchemeOption GetLinkStyle(string linkStyleString);
}
