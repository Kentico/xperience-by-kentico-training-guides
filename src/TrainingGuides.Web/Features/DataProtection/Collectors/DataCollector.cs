using CMS.DataEngine;
using CMS.DataProtection;
using TrainingGuides.Web.Features.DataProtection.Writers;

namespace TrainingGuides.Web.Features.DataProtection.Collectors;

public class DataCollector(IServiceProvider serviceProvider) : IPersonalDataCollector
{
    public PersonalDataCollectorResult Collect(IEnumerable<BaseInfo> identities, string outputFormat)
    {
        using var personalDataWriter = CreateWriter(outputFormat);

        var dataCollector = ActivatorUtilities.CreateInstance<DataCollectorCore>(serviceProvider, personalDataWriter);

        return new PersonalDataCollectorResult
        {
            Text = dataCollector.CollectData(identities)
        };
    }

    private IPersonalDataWriter CreateWriter(string outputFormat) =>
        outputFormat.ToLowerInvariant() switch
        {
            PersonalDataFormat.MACHINE_READABLE => new XmlPersonalDataWriter(),
            _ => new HumanReadablePersonalDataWriter()
        };
}
