using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.LandingPages;

public class LandingPageTemplateProperties : IPageTemplateProperties
{
    private const string DESCRIPTION = "Radio button group to select the type of HTML tag that wraps the home page message";

    /*
     * You can do this with enums as well, which can be a little easier
     * to manage in some cases
     * Ex implementation: https://github.com/Kentico/community-portal/blob/main/src/Kentico.Community.Portal.Web/Components/EnumDropDownOptionsProvider.cs#L7
     * Ex use case: https://github.com/Kentico/community-portal/blob/main/src/Kentico.Community.Portal.Web/Components/Widgets/CTAButton/CTAButtonWidget.cs#L45
     */
    private const string OPTIONS =
        "h1;Heading 1" + "\r\n" +
        "h2;Heading 2" + "\r\n" +
        "h3;Heading 3" + "\r\n" +
        "h4;Heading 4" + "\r\n" +
        "p;Paragraph" + "\r\n";

    private string messageType;

    [RadioGroupComponent(Label = "Message tag type", AriaLabel = DESCRIPTION, ExplanationText = DESCRIPTION, Inline = true, Options = OPTIONS)]
    public string MessageType
    {
        get => GetSafeTagText(messageType);
        set => messageType = value;
    }

    //ensures only valid strings will be rendered as raw html
    private string GetSafeTagText(string messageType) => messageType switch
    {
        "h1" => "h1",
        "h2" => "h2",
        "h3" => "h3",
        "h4" => "h4",
        "p" => "p",
        _ => "span"
    };
}

