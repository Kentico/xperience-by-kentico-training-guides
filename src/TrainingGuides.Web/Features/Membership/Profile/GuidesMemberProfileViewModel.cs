using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrainingGuides.Web.Features.Membership.Profile;

public class GuidesMemberProfileViewModel
{
    [DataType(DataType.Text)]
    [MaxLength(50)]
    [DisplayName("Given name")]
    public string GivenName { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [MaxLength(50)]
    [DisplayName("Family name")]
    public string FamilyName { get; set; } = string.Empty;

    [DisplayName("Family name goes first")]
    public bool FamilyNameFirst { get; set; } = false;

    [DataType(DataType.Text)]
    [MaxLength(100)]
    [DisplayName("Favorite coffee")]
    public string FavoriteCoffee { get; set; } = string.Empty;
}