using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Html.Widgets.HtmlCode;

public class HtmlCodeWidgetProperties : IWidgetProperties
{
    [TextAreaComponent(
        Label = "Code", MinRowsNumber = 10,
        Order = 10)]
    public string Code { get; set; } = null!;

    [CheckBoxComponent(
        Label = "Insert to page <head>",
        Order = 20)]
    public bool InsertToHead { get; set; }
}
