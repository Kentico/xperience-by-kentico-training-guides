using TrainingGuides.Web.Features.Shared.OptionProviders.CornerType;
using TrainingGuides.Web.Features.Shared.OptionsProviders.ColorScheme;

namespace TrainingGuides.Web.Features.Shared.Services;

public interface IComponentStyleEnumService
{
    IEnumerable<string> GetColorSchemeClasses(ColorSchemeOption colorScheme);

    IEnumerable<string> GetCornerTypeClasses(CornerTypeOption cornerType);

    CornerTypeOption GetCornerType(string cornerTypeString);

    ColorSchemeOption GetColorScheme(string colorSchemeString);
}
