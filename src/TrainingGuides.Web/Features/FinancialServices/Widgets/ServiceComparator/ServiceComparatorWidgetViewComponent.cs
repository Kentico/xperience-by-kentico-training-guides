using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.FinancialServices.Models;
using TrainingGuides.Web.Features.FinancialServices.Widgets.ServiceComparator;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.Services;

// NOTE: For an example of localizing widget name and description,
// see CallToActionWidgetViewComponent in Features/LandingPages/Widgets/CallToAction/

[assembly:
    RegisterWidget(
        identifier: ServiceComparatorWidgetViewComponent.IDENTIFIER,
        viewComponentType: typeof(ServiceComparatorWidgetViewComponent),
        name: "Service comparator",
        propertiesType: typeof(ServiceComparatorWidgetProperties),
        Description = "Displays a comparison between services.",
        IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.FinancialServices.Widgets.ServiceComparator;

public class ServiceComparatorWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ServiceComparatorWidget";

    private readonly IContentItemRetrieverService contentItemRetrieverService;
    private readonly IHttpRequestService httpRequestService;

    public ServiceComparatorWidgetViewComponent(
        IContentItemRetrieverService contentItemRetrieverService,
        IHttpRequestService httpRequestService)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.httpRequestService = httpRequestService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ServiceComparatorWidgetProperties properties)
    {
        var guids = properties.Services?.Select(i => i.Identifier).ToList() ?? [];

        var model = new ServiceComparatorWidgetViewModel()
        {
            Services = [],
            GroupedFeaturesHtmlDictionary = [],
            ComparatorHeading = properties.ComparatorHeading,
            HeadingType = properties.HeadingType,
            HeadingMargin = properties.HeadingMargin,
            ShowShortDescription = properties.ShowShortDescription,
            CheckboxIconUrl = $"{httpRequestService.GetBaseUrl()}/assets/img/icons.svg#check"
        };

        foreach (var guid in guids)
        {
            var service = await GetService(guid, properties);

            if (service != null)
            {
                model.Services.Add(service);

                model.GroupedFeaturesHtmlDictionary.AddRange(service.Features.Where(i => i.ShowInComparator)
                    .Select(feature => new KeyValuePair<string, HtmlString>(feature.Key, feature.LabelHtml)));
            }
        }

        model.GroupedFeaturesHtmlDictionary = model.GroupedFeaturesHtmlDictionary.DistinctBy(item => item.Key).ToList();

        return View("~/Features/FinancialServices/Widgets/ServiceComparator/ServiceComparatorWidget.cshtml", model);
    }

    private async Task<ServicePageViewModel?> GetService(Guid guid, ServiceComparatorWidgetProperties properties)
    {
        var servicePage = await contentItemRetrieverService.RetrieveWebPageByContentItemGuid<ServicePage>(
                            guid,
                            4);

        if (servicePage == null)
        {
            return new ServicePageViewModel
            {
                NameHtml = new("Error"),
                Features =
                [
                    new ServiceFeatureViewModel
                    {
                        Key = "error",
                        Name = "Error",
                        LabelHtml = new("Error"),
                        Price = 0,
                        ValueHtml = new("Unable to load service.<br/>Please double-check your page selection."),
                        FeatureIncluded = false,
                        ValueType = ServiceFeatureValueType.Text,
                        ShowInComparator = true,
                    }
                ]
            };
        }

        var service = servicePage.ServicePageService.FirstOrDefault();

        if (service == null)
            return null;

        var linkComponent = new LinkViewModel()
        {
            LinkUrl = servicePage.GetUrl().RelativePath,
            CallToAction = properties.CallToAction ?? string.Empty
        };

        var model = new ServicePageViewModel
        {
            NameHtml = new(service.ServiceName),
            ShortDescriptionHtml = new(service.ServiceShortDescription),
            Features = service.ServiceFeatures.Select(ServiceFeatureViewModel.GetViewModel).ToList(),
            Link = linkComponent,
            Price = service.ServicePrice
        };

        if (properties.ShowPrice)
        {
            model.Features.Add(
                new ServiceFeatureViewModel
                {
                    Key = "price-from-service-content-item",
                    Name = "Price",
                    LabelHtml = new("Price"),
                    Price = model.Price,
                    ValueHtml = new(string.Empty),
                    FeatureIncluded = false,
                    ValueType = ServiceFeatureValueType.Number,
                    ShowInComparator = true,
                });
        }

        return model;
    }
}
