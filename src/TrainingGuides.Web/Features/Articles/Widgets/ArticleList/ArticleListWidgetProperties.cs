using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetProperties : IWidgetProperties
{
    [ContentItemSelectorComponent(
        new[] {
            ArticlePage.CONTENT_TYPE_NAME,
            DownloadsPage.CONTENT_TYPE_NAME,
            EmptyPage.CONTENT_TYPE_NAME,
            LandingPage.CONTENT_TYPE_NAME,
            ProductPage.CONTENT_TYPE_NAME
        },
        Label = "Select a parent page to pull articles from",
        AllowContentItemCreation = false,
        MaximumItems = 1,
        Order = 10)]
    public IEnumerable<ContentItemReference> ContentTreeSection { get; set; } = Enumerable.Empty<ContentItemReference>();

    [NumberInputComponent(
        Label = "Number of articles to display",
        Order = 20)]
    public int TopN { get; set; } = 10;

    [TextInputComponent(
        Label = "CTA text",
        Order = 30)]
    public string CtaText { get; set; } = string.Empty;

    [DropDownComponent(
        Label = "Order articles by",
        Options = "NewestFirst;Newest first\r\nOldestFirst;Oldest first",
        Order = 40)]
    public string OrderBy { get; set; } = "NewestFirst";
}