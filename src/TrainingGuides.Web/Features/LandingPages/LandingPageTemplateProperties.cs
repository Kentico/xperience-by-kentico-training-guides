using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.LandingPages;

public class LandingPageTemplateProperties : IPageTemplateProperties
{
    private const string DESCRIPTION = "Select the type of Html tag that wraps the home page message.";

    private const string OPTIONS =
        "H1;Heading 1" + "\n" +
        "H2;Heading 2" + "\n" +
        "H3;Heading 3" + "\n" +
        "H4;Heading 4" + "\n" +
        "P;Paragraph" + "\n";

    [DropDownComponent(
        Label = "Message tag type",
        Options = OPTIONS,
        ExplanationText = DESCRIPTION)]
    public string MessageType { get; set; } = "H4";
}