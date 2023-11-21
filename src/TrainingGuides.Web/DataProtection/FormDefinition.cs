
using System.Collections.Generic;

namespace TrainingGuides.Web.DataProtection;

public class FormDefinition
{
    public const string FORM_CONSENT_COLUMN_NAME = "Consent";
    public IEnumerable<string> EmailColumns { get; }
    public List<CollectedColumn> FormColumns { get; }

    public FormDefinition(List<string> emailColumns, List<CollectedColumn> formColumns)
    {
        EmailColumns = emailColumns;
        FormColumns = formColumns;
    }
}
