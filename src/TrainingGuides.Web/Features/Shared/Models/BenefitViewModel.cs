using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.Shared.Models;

public class BenefitViewModel
{
    public HtmlString Description { get; set; } = HtmlString.Empty;
    public AssetViewModel Icon { get; set; } = new();

    public static BenefitViewModel GetViewModel(Benefit benefit) => new()
    {
        Description = new(benefit.BenefitDescription),
        Icon = benefit.BenefitIcon?.FirstOrDefault() != null
            ? AssetViewModel.GetViewModel(benefit.BenefitIcon.FirstOrDefault()!)
            : new(),
    };
}
