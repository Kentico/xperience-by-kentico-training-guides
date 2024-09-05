using CMS.DataEngine;
using CMS.Helpers;
using System.Text;
using System.Xml;
using TrainingGuides.Web.Features.DataProtection.Collectors;

namespace TrainingGuides.Web.Features.DataProtection.Writers;

public class XmlPersonalDataWriter : IPersonalDataWriter
{
    private readonly StringBuilder stringBuilder;
    private readonly XmlWriter xmlWriter;

    public XmlPersonalDataWriter()
    {
        stringBuilder = new StringBuilder();
        xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true });
    }

    public void WriteStartSection(string sectionName, string sectionDisplayName) =>
        xmlWriter.WriteStartElement(TransformElementName(sectionName));

    private string TransformElementName(string originalName) => originalName.Replace('.', '_');

    public void WriteBaseInfo(BaseInfo baseInfo, List<CollectedColumn> columns,
        Func<string, object, object>? valueTransformationFunction = null)
    {
        if (baseInfo == null)
        {
            throw new ArgumentNullException(nameof(baseInfo));
        }

        foreach (var columnTuple in columns)
        {
            string columnName = columnTuple.Name;

            if (string.IsNullOrWhiteSpace(columnTuple.DisplayName))
            {
                continue;
            }

            object value = baseInfo.GetValue(columnName);
            if (value == null)
            {
                continue;
            }

            if (valueTransformationFunction != null)
            {
                value = valueTransformationFunction(columnName, value);
            }

            xmlWriter.WriteStartElement(columnName);
            xmlWriter.WriteValue(XmlHelper.ConvertToString(value, value.GetType()));
            xmlWriter.WriteEndElement();
        }
    }

    public void WriteSectionValue(string sectionName, string sectionDisplayName, string value)
    {
        xmlWriter.WriteStartElement(sectionName);
        xmlWriter.WriteString(value);
        xmlWriter.WriteEndElement();
    }

    public void WriteEndSection() => xmlWriter.WriteEndElement();

    public string GetResult()
    {
        xmlWriter.Flush();

        return stringBuilder.ToString();
    }

    public void Dispose() => xmlWriter.Dispose();
}