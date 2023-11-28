using CMS.Base;
using CMS.DataEngine;
using System.Globalization;
using System.Text;
using TrainingGuides.Web.Features.DataProtection.Collectors;

namespace TrainingGuides.Web.Features.DataProtection.Writers;

public class HumanReadablePersonalDataWriter : IPersonalDataWriter
{
    private readonly StringBuilder stringBuilder;
    private int indentationLevel;
    private bool ignoreNewLine;

    private static readonly string decimalPrecision = new('#', 26);
    private static readonly string decimalFormat = "{0:0.00" + decimalPrecision + "}";

    public CultureInfo Culture { get; set; } = new CultureInfo(SystemContext.SYSTEM_CULTURE_NAME);

    public HumanReadablePersonalDataWriter()
    {
        stringBuilder = new StringBuilder();
        indentationLevel = 0;
        ignoreNewLine = false;
    }

    public void WriteStartSection(string sectionName, string sectionDisplayName)
    {
        ignoreNewLine = false;
        Indent();

        stringBuilder.AppendLine(sectionDisplayName + ": ");
        indentationLevel++;
    }

    private void Indent() => stringBuilder.Append('\t', indentationLevel);

    public void WriteBaseInfo(BaseInfo baseInfo, List<CollectedColumn> columns,
        Func<string, object, object> valueTransformationFunction = null)
    {
        if (baseInfo == null)
        {
            throw new ArgumentNullException(nameof(baseInfo));
        }

        foreach (var column in columns)
        {
            string columnName = column.Name;
            string columnDisplayName = column.DisplayName;

            if (string.IsNullOrWhiteSpace(columnDisplayName) ||
                columnName.Equals(baseInfo.TypeInfo.IDColumn, StringComparison.Ordinal) ||
                columnName.Equals(baseInfo.TypeInfo.GUIDColumn, StringComparison.Ordinal))
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

            WriteKeyValue(columnDisplayName, value);
        }
    }

    public void WriteSectionValue(string sectionName, string sectionDisplayName, string value)
    {
        Indent();

        stringBuilder.Append($"{sectionDisplayName}: {value}");
        stringBuilder.AppendLine();
    }

    private void WriteKeyValue(string keyName, object value)
    {
        Indent();
        stringBuilder.Append($"{keyName}: ");

        string format = "{0}";

        if (value is decimal)
        {
            format = decimalFormat;
        }

        stringBuilder.AppendFormat(Culture, format, value);
        stringBuilder.AppendLine();

        ignoreNewLine = true;
    }

    public void WriteEndSection()
    {
        indentationLevel--;

        if (ignoreNewLine)
            return;

        Indent();
        stringBuilder.AppendLine();
        ignoreNewLine = true;
    }

    public string GetResult() => stringBuilder.ToString();

    public void Dispose()
    {
    }
}