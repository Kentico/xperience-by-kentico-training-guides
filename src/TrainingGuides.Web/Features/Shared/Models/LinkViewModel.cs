namespace TrainingGuides.Web.Features.Shared.Models;
public class LinkViewModel
{
    public string Name { get; set; } = string.Empty;
    public string CallToAction { get; set; } = string.Empty;
    public string LinkTitleText { get; set; } = string.Empty;
    public string LinkUrl { get; set; } = string.Empty;
    public string LinkToExternal { get; set; } = string.Empty;
    public bool OpenInNewTab { get; set; }
}
