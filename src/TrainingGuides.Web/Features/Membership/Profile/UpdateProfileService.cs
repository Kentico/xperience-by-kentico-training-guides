using Kentico.Content.Web.Mvc.Routing;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Membership.Profile;

public class UpdateProfileService
{
    private readonly IHttpRequestService httpRequestService;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    private readonly IStringLocalizer<SharedResources> stringLocalizer;

    public UpdateProfileService(IHttpRequestService httpRequestService,
        IPreferredLanguageRetriever preferredLanguageRetriever,
        IStringLocalizer<SharedResources> stringLocalizer)
    {
        this.httpRequestService = httpRequestService;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
        this.stringLocalizer = stringLocalizer;
    }

    /// <inheritdoc/>
    public UpdateProfileViewModel GetViewModel(GuidesMember guidesMember) =>
        new()
        {
            GivenName = guidesMember?.GivenName ?? string.Empty,
            FamilyName = guidesMember?.FamilyName ?? string.Empty,
            FamilyNameFirst = guidesMember?.FamilyNameFirst ?? false,
            FavoriteCoffee = guidesMember?.FavoriteCoffee ?? string.Empty,
            EmailAddress = guidesMember?.Email ?? string.Empty,
            FullName = guidesMember?.FullName ?? string.Empty,
            Created = guidesMember?.Created ?? DateTime.MinValue,
            BaseUrl = httpRequestService.GetBaseUrl(),
            Language = preferredLanguageRetriever.Get(),
            SubmitButtonText = stringLocalizer["Submit"],
            SuccessMessage = string.Empty
        };
}
