using CMS.DataEngine;
using CMS.DataProtection;
using TrainingGuides.Web.Features.DataProtection.Writers;

namespace TrainingGuides.Web.Features.DataProtection.Collectors;

public class ContactDataCollector : IPersonalDataCollector
{
    public PersonalDataCollectorResult Collect(IEnumerable<BaseInfo> identities, string outputFormat)
    {
        using var writer = CreateWriter(outputFormat);

        // Activator.CreateInstance(typeof(DataCollectorCore))
        // ActivatorUtilities.CreateInstance(IServiceProvider, Type, Object[])
        var dataCollector = new DataCollectorCore(writer);

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
