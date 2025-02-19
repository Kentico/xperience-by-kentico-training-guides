using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Membership.Widgets.ResetPassword;

public class ResetPasswordWidgetViewModel : IWidgetViewModel
{
    public bool IsMisconfigured =>
        string.IsNullOrWhiteSpace(ActionUrl)
        || string.IsNullOrWhiteSpace(SubmitButtonText)
        || string.IsNullOrWhiteSpace(EmailAddressLabel);

    /// <summary>
    /// The Url of the controller action that the form should post to.
    /// </summary>
    public string ActionUrl { get; set; } = string.Empty;

    /// <summary>
    /// Submit button text
    /// </summary>
    [HiddenInput]
    public string SubmitButtonText { get; set; } = string.Empty;

    /// <summary>
    /// Email address label.
    /// </summary>
    [HiddenInput]
    public string EmailAddressLabel { get; set; } = string.Empty;

    /// <summary>
    /// Email address.
    /// </summary>
    [DataType(DataType.EmailAddress)]
    [Required()]
    [EmailAddress()]
    [MaxLength(254)]
    public string EmailAddress { get; set; } = string.Empty;
}