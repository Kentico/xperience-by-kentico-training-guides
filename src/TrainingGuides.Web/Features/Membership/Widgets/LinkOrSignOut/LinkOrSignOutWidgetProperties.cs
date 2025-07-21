using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;

namespace TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;

public class LinkOrSignOutWidgetProperties : IWidgetProperties
{
    [TextInputComponent(
        Label = "Unauthenticated text",
        ExplanationText = "Text to display when the visitor is not authenticated.",
        Order = 10)]
    public string UnauthenticatedText { get; set; } = string.Empty;

    [TextInputComponent(
        Label = "Unauthenticated link text",
        ExplanationText = "Text for the link button when the visitor is not authenticated.",
        Order = 20)]
    public string UnauthenticatedButtonText { get; set; } = string.Empty;

    [WebPageSelectorComponent(
        Label = "Unauthenticated link target page",
        ExplanationText = "Page to link to when the visitor is not authenticated.",
        MaximumPages = 1,
        Order = 30)]
    public IEnumerable<WebPageRelatedItem> UnauthenticatedTargetContentPage { get; set; } = [];

    [TextInputComponent(
        Label = "Authenticated text",
        ExplanationText = "Text to display when the visitor is authenticated.",
        Order = 40)]
    public string AuthenticatedText { get; set; } = string.Empty;

    [TextInputComponent(
    Label = "Authenticated button text",
    ExplanationText = "Text for the 'Sign out' link button when the visitor is authenticated.",
    Order = 50)]
    public string AuthenticatedButtonText { get; set; } = string.Empty;
}