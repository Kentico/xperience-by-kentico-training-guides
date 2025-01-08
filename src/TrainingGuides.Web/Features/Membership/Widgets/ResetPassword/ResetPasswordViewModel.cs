using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.Features.Membership.Widgets.ResetPassword;

public class ResetPasswordViewModel
{
    /// <summary>
    /// The action URL of the form
    /// </summary>
    public string ActionUrl { get; set; } = string.Empty;

    /// <summary>
    /// Determines whether the widget should display the form.
    /// </summary>
    [HiddenInput]
    public bool DisplayForm { get; set; }

    /// <summary>
    /// Title for the page and form.
    /// </summary>
    [HiddenInput]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Email address used to identify the member being reset.
    /// </summary>
    [HiddenInput]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Token used to verify the reset request.
    /// </summary>
    [HiddenInput]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Submit button text
    /// </summary>
    [HiddenInput]
    public string SubmitButtonText { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Required()]
    [Display(Name = "Password")]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Required()]
    [Display(Name = "Confirm your password")]
    [MaxLength(100)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

}