using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Shared.Sections.SingleColumn;

public class SingleColumnSectionProperties : ISectionProperties
{
    [TextInputComponent(
        Label = "Section anchor",
        Order = 10)]
    public string SectionAnchor { get; set; }
}