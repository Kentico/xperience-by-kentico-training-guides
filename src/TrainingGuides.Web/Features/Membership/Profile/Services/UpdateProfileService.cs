using Kentico.Content.Web.Mvc.Routing;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Shared.Helpers;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Membership.Profile;

public class UpdateProfileService : IUpdateProfileService
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
            UserName = guidesMember?.UserName ?? string.Empty,
            EmailAddress = guidesMember?.Email ?? string.Empty,
            FullName = guidesMember?.FullName ?? string.Empty,
            Created = guidesMember?.Created ?? DateTime.MinValue,
            ActionUrl = httpRequestService.GetAbsoluteUrlForPath(ApplicationConstants.UPDATE_PROFILE_ACTION_PATH, true),
            Language = preferredLanguageRetriever.Get(),
            SubmitButtonText = stringLocalizer["Submit"],
            Title = stringLocalizer["Update profile"]
        };
}
