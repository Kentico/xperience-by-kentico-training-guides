using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.Shared.Sections.FormColumn;

public class FormColumnSectionViewModel
{
    public bool ShowContents { get; set; }
    public string SectionAnchor { get; set; } = string.Empty;
    public HtmlString NoConsentHTML { get; set; } = new HtmlString(string.Empty);
}