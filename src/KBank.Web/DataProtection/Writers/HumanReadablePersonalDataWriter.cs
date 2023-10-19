using CMS.Base;
using CMS.DataEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KBank.Web.DataProtection.Writers;

public class HumanReadablePersonalDataWriter : IPersonalDataWriter
{
    private readonly StringBuilder stringBuilder;
    private int indentationLevel;
    private bool ignoreNewLine;

    private static readonly string DECIMAL_PRECISION = new string('#', 26);
    private static readonly string DECIMAL_FORMAT = "{0:0.00" + DECIMAL_PRECISION + "}";

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

    private void Indent()
    {
        stringBuilder.Append('\t', indentationLevel);
    }

    public void WriteBaseInfo(BaseInfo baseInfo, List<CollectedColumn> columns,
        Func<string, object, object> valueTransformationFunction = null)
    {
        if (baseInfo == null)
        {
            throw new ArgumentNullException(nameof(baseInfo));
        }

        foreach (var column in columns)
        {
            var columnName = column.Name;
            var columnDisplayName = column.DisplayName;

            if (string.IsNullOrWhiteSpace(columnDisplayName) ||
                columnName.Equals(baseInfo.TypeInfo.IDColumn, StringComparison.Ordinal) ||
                columnName.Equals(baseInfo.TypeInfo.GUIDColumn, StringComparison.Ordinal))
            {
                continue;
            }

            var value = baseInfo.GetValue(columnName);

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

        var format = "{0}";

        if (value is decimal)
        {
            format = DECIMAL_FORMAT;
        }

        stringBuilder.AppendFormat(Culture, format, value);
        stringBuilder.AppendLine();

        ignoreNewLine = true;
    }

    public void WriteEndSection()
    {
        indentationLevel--;

        if (ignoreNewLine) return;

        Indent();
        stringBuilder.AppendLine();
        ignoreNewLine = true;
    }

    public string GetResult() => stringBuilder.ToString();

    public void Dispose()
    {
    }
}