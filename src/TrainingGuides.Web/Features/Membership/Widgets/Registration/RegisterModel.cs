using System.ComponentModel.DataAnnotations;
using TrainingGuides.Web.Features.Membership.Widgets.Registration;

public class RegisterModel
{
    public RegistrationWidgetViewModel WidgetViewModel = new();

    [DataType(DataType.Text)]
    [Required()]
    [RegularExpression("^[a-zA-Z0-9_\\-\\.]+$")]
    [MaxLength(100)]
    public string UserName { get; set; } = "";

    [DataType(DataType.EmailAddress)]
    [Required()]
    [EmailAddress()]
    [MaxLength(100)]
    public string EmailAddress { get; set; } = "";

    [DataType(DataType.Password)]
    [Required()]
    [MaxLength(100)]
    public string Password { get; set; } = "";

    [DataType(DataType.Password)]
    [Required()]
    [MaxLength(100)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = "";

    [DataType(DataType.Text)]
    [MaxLength(100)]
    public string GivenName { get; set; } = "";

    [DataType(DataType.Text)]
    [MaxLength(100)]
    public string FamilyName { get; set; } = "";

    public bool FamilyNameFirst { get; set; } = false;

    [DataType(DataType.Text)]
    [MaxLength(100)]
    public string FavoriteCoffee { get; set; } = "";
}