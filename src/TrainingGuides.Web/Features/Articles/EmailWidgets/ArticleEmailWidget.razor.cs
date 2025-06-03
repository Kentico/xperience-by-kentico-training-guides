
using Microsoft.AspNetCore.Components;
using TrainingGuides.Web.Features.Articles.EmailWidgets;
using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;

// [assembly: RegisterEmailWidget(
//     identifier: ArticleEmailWidget.IDENTIFIER,
//     name: "Article",
//     componentType: typeof(ArticleEmailWidget),
//     PropertiesType = typeof(ArticleEmailWidgetProperties),
//     IconClass = "icon-l-list-img-article",
//     Description = "Displays an article teaser link with an image, summary, and title."
//     )]

namespace TrainingGuides.Web.Features.Articles.EmailWidgets;

public partial class ArticleEmailWidget : ComponentBase
{
    /// <summary>
    /// The component identifier.
    /// </summary>
    public const string IDENTIFIER = $"TrainingGuides.{nameof(ArticleEmailWidget)}";

    [Inject]
    private IComponentModelMapper<ArticleEmailWidgetModel> ArticleComponentModelMapper { get; set; } = default!;

    [Inject]
    private IEmailContextAccessor EmailContextAccessor { get; set; } = default!;

    /// <summary>
    /// The widget properties.
    /// </summary>
    [Parameter]
    public ArticleEmailWidgetProperties Properties { get; set; } = new();

    public ArticleEmailWidgetModel Model { get; set; } = new();

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        var itemGuid = Properties.ArticlePage.FirstOrDefault()?.Identifier;

        string languageName = EmailContextAccessor.GetContext().LanguageName;

        Model = await ArticleComponentModelMapper.Map(itemGuid ?? Guid.Empty, languageName);
    }
}