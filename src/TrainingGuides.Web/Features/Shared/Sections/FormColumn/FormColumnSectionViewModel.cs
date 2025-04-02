namespace TrainingGuides.Web.Features.Shared.Sections.FormColumn;

public class FormColumnSectionViewModel
{
    public bool ShowContents { get; set; }
    public string SectionAnchor { get; set; } = string.Empty;
    public string NoConsentMessage { get; set; } = string.Empty;
    public string NoConsentLinkText { get; set; } = string.Empty;
    public string NoConsentLinkUrl { get; set; } = string.Empty;
}
