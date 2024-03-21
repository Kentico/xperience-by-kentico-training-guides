using Kentico.PageBuilder.Web.Mvc;

namespace TrainingGuides.Web.Features.Shared.ViewComponents;

public class PageBuilderColumnViewModel
{
    public string? CssClass { get; set; }
    public string? Identifier { get; set; }
    public EditableAreaOptions? EditableAreaOptions { get; set; }
}
