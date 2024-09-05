using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;

namespace TrainingGuides.Web.Features.Shared.Sections.General;

public class GeneralSectionViewModel
{
    public string SectionAnchor { get; set; } = string.Empty;
    public string ColorScheme { get; set; } = string.Empty;
    public string CornerStyle { get; set; } = string.Empty;
    public ColumnLayoutOption ColumnLayout { get; set; }
}
