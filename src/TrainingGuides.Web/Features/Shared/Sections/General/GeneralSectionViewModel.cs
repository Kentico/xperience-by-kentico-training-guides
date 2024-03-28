using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;

namespace TrainingGuides.Web.Features.Shared.Sections.General;

public class GeneralSectionViewModel
{
    public string? SectionAnchor { get; set; }
    public string? ColorScheme { get; set; }
    public string? CornerStyle { get; set; }
    public ColumnLayoutOption ColumnLayout { get; set; }
}
