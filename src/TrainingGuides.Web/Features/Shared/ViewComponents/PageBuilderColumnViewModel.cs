using Kentico.PageBuilder.Web.Mvc;

namespace TrainingGuides.Web.Features.Shared.ViewComponents;

public class PageBuilderColumnViewModel
{
    public PageBuilderAreaType AreaType { get; set; }
    public string CssClass { get; set; }
    public string Identifier { get; set; }
    public EditableAreaOptions? EditableAreaOptions { get; set; }
}
