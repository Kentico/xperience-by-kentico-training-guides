using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.Shared.Models;

public class BenefitViewModel
{
    public HtmlString DescriptionHtml { get; set; } = HtmlString.Empty;
    public AssetViewModel Icon { get; set; } = new();

    public static BenefitViewModel GetViewModel(Benefit benefit) => new()
    {
        DescriptionHtml = new(benefit.BenefitDescription),
        Icon = benefit.BenefitIcon?.FirstOrDefault() != null
            ? AssetViewModel.GetViewModel(benefit.BenefitIcon.FirstOrDefault())
            : new(),
    };
}
