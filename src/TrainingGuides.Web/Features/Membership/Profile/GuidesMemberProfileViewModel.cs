using System.ComponentModel.DataAnnotations;

namespace TrainingGuides.Web.Features.Membership.Profile;

public class GuidesMemberProfileViewModel
{
    [DataType(DataType.Text)]
    [MaxLength(50)]
    [Display(Name = "Given name")]
    public string GivenName { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [MaxLength(50)]
    [Display(Name = "Family name")]
    public string FamilyName { get; set; } = string.Empty;

    [Display(Name = "Family name goes first")]
    public bool FamilyNameFirst { get; set; } = false;

    [DataType(DataType.Text)]
    [MaxLength(100)]
    [Display(Name = "Favorite coffee")]
    public string FavoriteCoffee { get; set; } = string.Empty;
}