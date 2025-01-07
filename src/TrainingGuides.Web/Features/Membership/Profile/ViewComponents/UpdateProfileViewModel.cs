using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.Features.Membership.Profile;

public class UpdateProfileViewModel : GuidesMemberProfileViewModel
{
    private string fullName = string.Empty;

    [DisplayName("Full name")]
    public string FullName
    {
        get => !string.IsNullOrWhiteSpace(fullName)
            ? fullName
            : FamilyNameFirst
                ? $"{FamilyName} {GivenName}"
                : $"{GivenName} {FamilyName}";
        set => fullName = value ?? string.Empty;
    }

    [DisplayName("Email address")]
    public string EmailAddress { get; set; } = string.Empty;

    [DisplayName("User name")]
    public string UserName { get; set; } = string.Empty;

    [DisplayName("Member since")]
    public DateTime Created { get; set; }

    [HiddenInput]
    public string SubmitButtonText { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string ActionUrl { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;

    public string SuccessMessage { get; set; } = string.Empty;
}
