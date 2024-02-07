using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.HTML.Widgets.HtmlCode;

public class HtmlCodeWidgetProperties : IWidgetProperties
{
    [TextAreaComponent(Order = 0, Label = "Code", MinRowsNumber = 10)]
    public string Code { get; set; } = null!;

    [CheckBoxComponent(Order = 1, Label = "Insert to page <head>")]
    public bool InsertToHead { get; set; }
}
