namespace TrainingGuides.Web.Features.Shared.Services;

public interface ICountryService
{
    IEnumerable<string> GetCountriesByGuids(IEnumerable<Guid> countryGuids);
}
