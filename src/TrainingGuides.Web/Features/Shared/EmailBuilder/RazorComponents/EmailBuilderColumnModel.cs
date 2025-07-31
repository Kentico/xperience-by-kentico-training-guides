namespace TrainingGuides.Web.Features.Shared.EmailBuilder.RazorComponents;

public class EmailBuilderColumnModel
{
    public int Width { get; set; } = 0;
    public string Identifier { get; set; } = string.Empty;
    public IEnumerable<string> CssClasses { get; set; } = [];
    public IEnumerable<string> AllowedWidgets { get; set; } = [];
    // If you want to apply this approach to Editable Areas rather than Widget Zones, you can also include this:
    // public IEnumerable<string> AllowedSections { get; set; } = [];
}