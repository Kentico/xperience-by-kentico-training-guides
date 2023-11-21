using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Components.Sections;

public class FormColumnSectionProperties : ISectionProperties
{
    [TextInputComponent(Label = "Section anchor", Order = 1)]
    public string SectionAnchor { get; set; }
}