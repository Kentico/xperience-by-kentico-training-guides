using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.HTML.Widgets.HtmlCode;

public class HtmlCodeWidgetViewModel : WidgetViewModel
{
    public HtmlString? Code { get; set; }

    public override bool IsMisconfigured => false;
}
