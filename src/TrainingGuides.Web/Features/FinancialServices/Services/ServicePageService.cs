using TrainingGuides.Web.Features.FinancialServices.Models;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.FinancialServices.Services;

public class ServicePageService(IWebPageUrlRetriever webPageUrlRetriever) : IServicePageService
{
    /// <summary>
    /// Creates a new instance of <see cref="ServicePageViewModel"/>, setting the properties using ServicePage given as a parameter.
    /// </summary>
    /// <param name="servicePage">Corresponding Service page object.</param>
    /// <returns>New instance of ServicePageViewModel.</returns>
    public async Task<ServicePageViewModel> GetServicePageViewModel(
        ServicePage? servicePage,
        bool getMedia = true,
        bool getFeatures = true,
        bool getBenefits = true,
        string callToActionText = "",
        string callToActionLink = "",
        bool openInNewTab = true,
        bool getPrice = true)
    {
        if (servicePage == null)
        {
            return new ServicePageViewModel();
        }

        string url = string.IsNullOrWhiteSpace(callToActionLink)
            ? (await webPageUrlRetriever.Retrieve(servicePage)).RelativePath
            : callToActionLink;
        return new ServicePageViewModel
        {
            NameHtml = new(servicePage.ServicePageService.FirstOrDefault()?.ServiceName),
            ShortDescriptionHtml = new(servicePage.ServicePageService.FirstOrDefault()?.ServiceShortDescription),
            DescriptionHtml = new(servicePage.ServicePageService.FirstOrDefault()?.ServiceDescription),
            Media = getMedia
                ? servicePage.ServicePageService.FirstOrDefault()?.ServiceMedia.Select(AssetViewModel.GetViewModel)?.ToList() ?? []
                : [],
            Link = new LinkViewModel()
            {
                Name = servicePage.ServicePageService.FirstOrDefault()?.ServiceName ?? string.Empty,
                LinkUrl = url,
                CallToAction = callToActionText,
                OpenInNewTab = openInNewTab
            },
            Features = getFeatures
                ? servicePage.ServicePageService.FirstOrDefault()?.ServiceFeatures
                    .Select(ServiceFeatureViewModel.GetViewModel)
                    .ToList() ?? []
                : [],
            Benefits = getBenefits
                ? servicePage.ServicePageService.FirstOrDefault()?.ServiceBenefits
                    .Select(BenefitViewModel.GetViewModel)
                    .ToList() ?? []
                : [],
            Price = getPrice ? servicePage.ServicePageService.FirstOrDefault()?.ServicePrice ?? 0 : 0,
        };
    }
}