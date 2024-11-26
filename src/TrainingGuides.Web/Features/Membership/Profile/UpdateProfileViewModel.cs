using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.Features.Membership.Profile;

public class UpdateProfileViewModel : GuidesMemberProfileViewModel
{
    public string FullName { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public string BaseUrl { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;

    public string SuccessMessage { get; set; } = string.Empty;

    [HiddenInput]
    public string SubmitButtonText { get; set; } = string.Empty;

}
