using EnumsNET;

namespace TrainingGuides.Web.Features.Shared.OptionProviders;

public class EnumStringService : IEnumStringService
{
    public T Parse<T>(string value, T defaultValue) where T : struct, Enum =>
        Enums.TryParse<T>(value, true, out var parsed)
            ? parsed
            : defaultValue;
}
