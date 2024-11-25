using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Membership.Widgets.ResetPassword;

public class ResetPasswordWidgetViewModel : WidgetViewModel
{
    public override bool IsMisconfigured =>
        string.IsNullOrWhiteSpace(BaseUrlWithLanguage)
        || string.IsNullOrWhiteSpace(SubmitButtonText)
        || string.IsNullOrWhiteSpace(EmailAddressLabel);

    /// <summary>
    /// The Base URL of the site, with language
    /// </summary>
    [HiddenInput]
    public string BaseUrlWithLanguage { get; set; } = string.Empty;

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

    [DataType(DataType.EmailAddress)]
    [Required()]
    [EmailAddress()]
    [MaxLength(100)]
    public string EmailAddress { get; set; } = string.Empty;
}