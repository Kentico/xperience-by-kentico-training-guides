using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace KBank.Web.Components.PageTemplates;

public class HomePageTemplateProperties : IPageTemplateProperties
{
    private const string DESCRIPTION = "Radio button group to select the type of HTML tag that wraps the home page message";

    private const string OPTIONS =
        "h1;Heading 1" + "\r\n" +
        "h2;Heading 2" + "\r\n" +
        "h3;Heading 3" + "\r\n" +
        "h4;Heading 4" + "\r\n" +
        "p;Paragraph" + "\r\n";

    [RadioGroupComponent(Label = "Message tag type", AriaLabel = DESCRIPTION, ExplanationText = DESCRIPTION, Inline = true, Options = OPTIONS)]
    public string MessageType { get; set; }
}

