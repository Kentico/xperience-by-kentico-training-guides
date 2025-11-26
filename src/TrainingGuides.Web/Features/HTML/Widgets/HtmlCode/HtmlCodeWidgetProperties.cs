using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Html.Widgets.HtmlCode;

// NOTE: For an example of localizing widget properties (labels, explanation texts, and options),
// see CallToActionWidgetProperties in Features/LandingPages/Widgets/CallToAction/
public class HtmlCodeWidgetProperties : IWidgetProperties
{
    [TextAreaComponent(
        Label = "Code", MinRowsNumber = 10,
        Order = 10)]
    public string Code { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "Insert to page <head>",
        Order = 20)]
    public bool InsertToHead { get; set; }
}
