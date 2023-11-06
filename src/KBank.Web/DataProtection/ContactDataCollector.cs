using CMS.DataEngine;
using CMS.DataProtection;
using KBank.Web.DataProtection.Writers;
using System.Collections.Generic;

namespace KBank.Web.DataProtection;

public class ContactDataCollector : IPersonalDataCollector
{
    public PersonalDataCollectorResult Collect(IEnumerable<BaseInfo> identities, string outputFormat)
    {
        using var writer = CreateWriter(outputFormat);
        var dataCollector = new DataCollectorCore(writer);

        return new PersonalDataCollectorResult
        {
            Text = dataCollector.CollectData(identities)
        };
    }

    private IPersonalDataWriter CreateWriter(string outputFormat)
    {
        return outputFormat.ToLowerInvariant() switch
        {
            PersonalDataFormat.MACHINE_READABLE => new XmlPersonalDataWriter(),
            _ => new HumanReadablePersonalDataWriter()
        };
    }
}
