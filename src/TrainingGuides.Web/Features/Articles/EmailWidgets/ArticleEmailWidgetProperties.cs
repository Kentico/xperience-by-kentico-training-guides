using CMS.ContentEngine;

using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

namespace TrainingGuides.Web.Features.Articles.EmailWidgets;

/// <summary>
/// Configurable properties of the <see cref="ContentWidget"/>.
/// </summary>
public sealed class ArticleEmailWidgetProperties : IEmailWidgetProperties
{
    [ContentItemSelectorComponent(
        contentTypeFilter: typeof(ArticleContentTypesFilter),
        Label = "Select article",
        ExplanationTextAsHtml = true,
        ExplanationText = "The widget will display the content from the selected article.",
        Order = 10,
        MaximumItems = 1)]
    public IEnumerable<ContentItemReference> ArticlePage { get; set; } = [];
}
