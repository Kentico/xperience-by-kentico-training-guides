using CMS.DataEngine;
using CMS.Globalization;

namespace TrainingGuides.Web.Features.Shared.Services;

public class CountryService : ICountryService
{
    private readonly IInfoProvider<CountryInfo> countryInfoProvider;

    public CountryService(IInfoProvider<CountryInfo> countryInfoProvider)
    {
        this.countryInfoProvider = countryInfoProvider;
    }

    public IEnumerable<string> GetCountryDisplayNamesByGuids(IEnumerable<Guid> countryGuids) => countryInfoProvider.Get()
            .WhereIn(nameof(CountryInfo.CountryGUID), countryGuids)
            .Column(nameof(CountryInfo.CountryDisplayName))
            .GetListResult<string>();
}
