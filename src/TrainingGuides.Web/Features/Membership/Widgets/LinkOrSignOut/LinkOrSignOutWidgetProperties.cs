using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

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

    [ContentItemSelectorComponent(
        [
            ArticlePage.CONTENT_TYPE_NAME,
            DownloadsPage.CONTENT_TYPE_NAME,
            EmptyPage.CONTENT_TYPE_NAME,
            LandingPage.CONTENT_TYPE_NAME,
            ProductPage.CONTENT_TYPE_NAME,
            ProfilePage.CONTENT_TYPE_NAME
        ],
        Label = "Unauthenticated link target page",
        ExplanationText = "Page to link to when the visitor is not authenticated.",
        MaximumItems = 1,
        Order = 30)]
    public IEnumerable<ContentItemReference> UnauthenticatedTargetContentPage { get; set; } = Enumerable.Empty<ContentItemReference>();

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