using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TrainingGuides.Web.Extensions;

public static class HtmlHelperExtensions
{
    public static IHtmlContent? FormatDate(this IHtmlHelper helper, DateTime date) =>
        date.Year > 1900 ? new HtmlString(date.ToString("d MMMM yyyy")) : null;
}