using Kentico.PageBuilder.Web.Mvc;

namespace TrainingGuides.Web.Features.Shared.ViewComponents;

public class PageBuilderColumnViewModel
{
    public string CssClass { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public EditableAreaOptions? EditableAreaOptions { get; set; }
}
