using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.Features.Membership.Profile;

public class UpdateProfileViewModel : GuidesMemberProfileViewModel
{
    public string Title { get; set; } = string.Empty;

    [DisplayName("Full name")]
    public string FullName { get; set; } = string.Empty;

    [DisplayName("Email address")]
    public string EmailAddress { get; set; } = string.Empty;

    [DisplayName("Member since")]
    public DateTime Created { get; set; }

    public string BaseUrl { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;

    public string SuccessMessage { get; set; } = string.Empty;

    [HiddenInput]
    public string SubmitButtonText { get; set; } = string.Empty;

}
