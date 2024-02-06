using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.Shared.Models;

public class BenefitViewModel
{
    public string? Name { get; set; }
    public HtmlString? Description { get; set; }
    public AssetViewModel? Asset { get; set; }
    public string? BenefitTypes { get; set; }
    public string? ListItemType { get; set; }

    public static BenefitViewModel GetViewModel(Benefit benefit) => new()
    {
        Description = new(benefit.BenefitDescription),
        //I fixed the indentation and added nulll forgiving operator to get rid of the warning
        Asset = benefit.BenefitIcon?.FirstOrDefault() != null
            ? AssetViewModel.GetViewModel(benefit.BenefitIcon.FirstOrDefault()!)
            : null,
    };
}
