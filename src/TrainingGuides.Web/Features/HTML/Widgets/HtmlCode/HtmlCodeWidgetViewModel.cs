using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Html.Widgets.HtmlCode;

public class HtmlCodeWidgetViewModel : WidgetViewModel
{
    public HtmlString Code { get; set; } = HtmlString.Empty;

    public override bool IsMisconfigured => string.IsNullOrEmpty(Code.Value);
}
