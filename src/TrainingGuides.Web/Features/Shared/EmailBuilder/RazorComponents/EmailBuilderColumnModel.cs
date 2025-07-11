namespace TrainingGuides.Web.Features.Shared.EmailBuilder.RazorComponents;

public class EmailBuilderColumnModel
{
    public int Width { get; set; } = 0;
    public string Identifier { get; set; } = string.Empty;
    public IEnumerable<string> CssClasses { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> AllowedWidgets { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> AllowedSections { get; set; } = Enumerable.Empty<string>();
}