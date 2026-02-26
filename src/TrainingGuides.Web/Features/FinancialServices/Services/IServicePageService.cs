using TrainingGuides.Web.Features.FinancialServices.Models;

namespace TrainingGuides.Web.Features.FinancialServices.Services;

public interface IServicePageService
{
    Task<ServicePageViewModel> GetServicePageViewModel(
        ServicePage? servicePage,
        bool getMedia = true,
        bool getFeatures = true,
        bool getBenefits = true,
        string callToAction = "",
        string callToActionLink = "",
        bool openInNewTab = true,
        bool getPrice = true);
}