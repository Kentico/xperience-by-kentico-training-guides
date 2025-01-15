﻿using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Html.Widgets.HtmlCode;

public class HtmlCodeWidgetViewModel : IWidgetViewModel
{
    public HtmlString Code { get; set; } = HtmlString.Empty;

    public bool IsMisconfigured => string.IsNullOrEmpty(Code.Value);
}
