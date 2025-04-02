using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.CodeSnippets;

public class CodeSnippetViewModel
{
    public HtmlString CodeSnippetHtml { get; set; } = new HtmlString(string.Empty);
    public string CodeSnippetType { get; set; } = string.Empty;
    public string CodeSnippetLabel { get; set; } = string.Empty;
}
