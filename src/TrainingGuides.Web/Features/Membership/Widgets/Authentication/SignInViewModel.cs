using System.ComponentModel.DataAnnotations;
using TrainingGuides.Web.Features.Membership.Widgets.Authentication;

namespace TrainingGuides.Web.Features.Widgets.Authentication;

public class SignInViewModel
{
    [Required(ErrorMessage = "Please enter your user name or email address")]
    [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
    public string Password { get; set; } = string.Empty;

    public bool StaySignedIn { get; set; } = false;

    public SignInWidgetViewModel Labels { get; set; } = new();
}
