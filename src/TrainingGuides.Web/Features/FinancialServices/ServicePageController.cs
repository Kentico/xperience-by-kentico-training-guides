using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides;
using TrainingGuides.Web.Features.FinancialServices.Models;
using TrainingGuides.Web.Features.FinancialServices.Services;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWebPageRoute(
    contentTypeName: ServicePage.CONTENT_TYPE_NAME,
    controllerType: typeof(TrainingGuides.Web.Features.FinancialServices.ServicePageController))]

namespace TrainingGuides.Web.Features.FinancialServices;

public class ServicePageController : Controller
{
    private readonly IContentItemRetrieverService contentItemRetriever;
    private readonly IServicePageService servicePageService;

    public ServicePageController(
        IContentItemRetrieverService contentItemRetriever,
        IServicePageService servicePageService)
    {
        this.contentItemRetriever = contentItemRetriever;
        this.servicePageService = servicePageService;
    }

    public async Task<IActionResult> Index()
    {
        var servicePage = await contentItemRetriever.RetrieveCurrentPage<ServicePage>(3);

        var model = await servicePageService.GetServicePageViewModel(servicePage);
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

        return new TemplateResult(model);
    }
}
