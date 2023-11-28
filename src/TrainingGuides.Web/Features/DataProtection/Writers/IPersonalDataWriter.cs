using CMS.DataEngine;
using TrainingGuides.Web.Features.DataProtection.Collectors;

namespace TrainingGuides.Web.Features.DataProtection.Writers;

public interface IPersonalDataWriter : IDisposable
{
    void WriteStartSection(string sectionName, string sectionDisplayName);
    void WriteBaseInfo(BaseInfo baseInfo, List<CollectedColumn> columns, Func<string, object, object> valueTransformationFunction = null);
    void WriteSectionValue(string sectionName, string sectionDisplayName, string value);
    void WriteEndSection();
    string GetResult();
}