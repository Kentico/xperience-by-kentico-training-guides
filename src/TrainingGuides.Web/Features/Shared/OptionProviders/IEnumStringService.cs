namespace TrainingGuides.Web.Features.Shared.OptionProviders;

public interface IEnumStringService
{
    T Parse<T>(string value, T defaultValue) where T : struct, Enum;
}
