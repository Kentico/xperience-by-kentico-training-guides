namespace TrainingGuides.Web.Features.Shared.Services;

public interface ICountryService
{
    IEnumerable<string> GetCountryDisplayNamesByGuids(IEnumerable<Guid> countryGuids);
}
