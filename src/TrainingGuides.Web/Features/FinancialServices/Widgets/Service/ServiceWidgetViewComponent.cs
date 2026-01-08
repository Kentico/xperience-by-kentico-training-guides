using CMS.Helpers;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.FinancialServices.Models;
using TrainingGuides.Web.Features.FinancialServices.Services;
using TrainingGuides.Web.Features.FinancialServices.Widgets.Service;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.Services;

// NOTE: For an example of localizing widget name and description,
// see CallToActionWidgetViewComponent in Features/LandingPages/Widgets/CallToAction/

[assembly: RegisterWidget(
    identifier: FinancialServiceWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(FinancialServiceWidgetViewComponent),
    name: "Service",
    propertiesType: typeof(ServiceWidgetProperties),
    Description = "Displays selected service.",
    IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.FinancialServices.Widgets.Service;

public class FinancialServiceWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ServiceWidget";
    private const string BS_DROP_SHADOW_CLASS = "shadow";
    private const string BS_MARGIN_CLASS = "m-3";
    private const string BS_PADDING_CLASS_3 = "p-3";
    private const string BS_PADDING_CLASS_5 = "p-5";

    private readonly IContentItemRetrieverService contentItemRetrieverService;
    private readonly IComponentStyleEnumService componentStyleEnumService;
    private readonly IServicePageService servicePageService;
    private readonly IContentTypeService contentTypeService;

    public FinancialServiceWidgetViewComponent(
        IContentItemRetrieverService contentItemRetrieverService,
        IComponentStyleEnumService componentStyleEnumService,
        IServicePageService servicePageService,
        IContentTypeService contentTypeService)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.componentStyleEnumService = componentStyleEnumService;
        this.servicePageService = servicePageService;
        this.contentTypeService = contentTypeService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ServiceWidgetProperties properties)
    {
        var model = await GetServiceWidgetViewModel(properties);
        return View("~/Features/Services/Widgets/Service/ServiceWidget.cshtml", model);
    }

    private async Task<ServiceWidgetViewModel> GetServiceWidgetViewModel(ServiceWidgetProperties properties)
    {
        if (properties == null)
            return new ServiceWidgetViewModel();

        var servicePageViewModel = await GetServicePageViewModel(properties);

        return new ServiceWidgetViewModel()
        {
            Service = servicePageViewModel,
            ShowServiceFeatures = properties.ShowServiceFeatures,
            ServiceImage = properties.ShowServiceImage
                ? servicePageViewModel?.Media.FirstOrDefault() ?? new AssetViewModel()
                : new AssetViewModel(),
            ShowAdvanced = properties.ShowAdvanced,
            ColorScheme = properties.ColorScheme,
            CornerStyle = properties.CornerStyle,
            ParentElementCssClasses = GetParentElementCssClasses(properties).Join(" "),
            MainContentElementCssClasses = GetMainContentElementCssClasses(properties).Join(" "),
            ImageElementCssClasses = properties.ShowServiceImage ? GetImageElementCssClasses(properties).Join(" ") : string.Empty,
            IsImagePositionSide = IsImagePositionSide(properties.ImagePosition),
            CallToActionCssClasses = componentStyleEnumService
                .GetColorSchemeClasses(componentStyleEnumService.GetLinkStyle(properties.CallToActionStyle ?? string.Empty))
                .Join(" ")
        };
    }

    private async Task<ServicePage?> GetServicePage(ServiceWidgetProperties properties)
    {
        ServicePage? servicePage;

        if (properties.Mode.Equals(ServiceWidgetModel.CURRENT_PAGE))
        {
            servicePage = await contentItemRetrieverService.RetrieveCurrentPage<ServicePage>(3);
        }
        else
        {
            var guid = properties.SelectedServicePage?.Select(webPage => webPage.Identifier).FirstOrDefault();

            servicePage = guid.HasValue
                ? await contentItemRetrieverService.RetrieveWebPageByContentItemGuid<ServicePage>(
                    (Guid)guid,
                    4)
                : null;
        }

        int? serviceContentTypeId = contentTypeService.GetContentTypeId(ServicePage.CONTENT_TYPE_NAME);

        return servicePage?.SystemFields.ContentItemContentTypeID == serviceContentTypeId
            ? servicePage
            : null;
    }

    private async Task<ServicePageViewModel?> GetServicePageViewModel(ServiceWidgetProperties properties)
    {
        var servicePage = await GetServicePage(properties);

        if (!string.IsNullOrWhiteSpace(properties.PageAnchor))
        {
            properties.PageAnchor = properties.PageAnchor!.StartsWith('#')
                ? properties.PageAnchor
                : $"#{properties.PageAnchor}";
        }

        return servicePage != null
            ? await servicePageService.GetServicePageViewModel(
                servicePage: servicePage,
                getMedia: properties.ShowServiceImage,
                getFeatures: properties.ShowServiceFeatures,
                getBenefits: properties.ShowServiceBenefits,
                callToAction: properties.CallToAction,
                callToActionLink: properties.PageAnchor,
                openInNewTab: properties.OpenInNewTab,
                getPrice: properties.ShowServiceFeatures)
            : null;
    }

    private List<string> GetParentElementCssClasses(ServiceWidgetProperties properties)
    {
        const string PARENT_ELEMENT_BASE_CLASS = "tg-product";
        const string LAYOUT_FULL_WIDTH_CLASS = "tg-layout-full-width";
        const string LAYOUT_IMAGE_LEFT_CLASS = "tg-layout-image-left";
        const string LAYOUT_IMAGE_RIGHT_CLASS = "tg-layout-image-right";
        const string LAYOUT_ASCENDING_CLASS = "tg-layout-ascending";
        const string LAYOUT_DESCENDING_CLASS = "tg-layout-descending";

        List<string> cssClasses = [PARENT_ELEMENT_BASE_CLASS];

        if (properties.ShowServiceImage)
        {
            string imagePositionCssClass = properties.ImagePosition switch
            {
                nameof(ImagePositionOption.Left) => LAYOUT_IMAGE_LEFT_CLASS,
                nameof(ImagePositionOption.Right) => LAYOUT_IMAGE_RIGHT_CLASS,
                nameof(ImagePositionOption.Ascending) => LAYOUT_ASCENDING_CLASS,
                nameof(ImagePositionOption.Descending) => LAYOUT_DESCENDING_CLASS,
                nameof(ImagePositionOption.FullWidth) => LAYOUT_FULL_WIDTH_CLASS,
                _ => LAYOUT_FULL_WIDTH_CLASS
            };
            cssClasses.Add(imagePositionCssClass);

            if (IsImagePositionFullSize(properties.ImagePosition))
            {
                cssClasses.Add(BS_MARGIN_CLASS);

                cssClasses.AddRange(
                    componentStyleEnumService.GetCornerStyleClasses(
                        componentStyleEnumService.GetCornerStyle(properties.CornerStyle!)));

                if (properties.DropShadow)
                    cssClasses.Add(BS_DROP_SHADOW_CLASS);
            }
        }

        return cssClasses;
    }

    private List<string> GetMainContentElementCssClasses(ServiceWidgetProperties properties)
    {
        const string MAIN_CONTENT_BASE_CSS_CLASS = "tg-product_main";
        const string ROUND_CORNERS_BOTTOM_ONLY_CLASS = "bottom-only";
        const string TEXT_ALIGN_LEFT_CLASS = "align-left";
        const string TEXT_ALIGN_CENTER_CLASS = "align-center";
        const string TEXT_ALIGN_RIGHT_CLASS = "align-right";

        List<string> cssClasses = [MAIN_CONTENT_BASE_CSS_CLASS];

        cssClasses.AddRange(GetChildElementCssClasses(properties));

        if (properties.ShowServiceImage)
        {
            if (IsImagePositionSide(properties.ImagePosition))
                cssClasses.Add(BS_PADDING_CLASS_5);
            else
                cssClasses.Add(BS_PADDING_CLASS_3);

            if (IsImagePositionFullSize(properties.ImagePosition)
                && HasRoundCorners(properties.CornerStyle))
            {
                cssClasses.Add(ROUND_CORNERS_BOTTOM_ONLY_CLASS);
            }
        }

        string textAlignmentClass = properties.TextAlignment switch
        {
            nameof(ContentAlignmentOption.Left) => TEXT_ALIGN_LEFT_CLASS,
            nameof(ContentAlignmentOption.Center) => TEXT_ALIGN_CENTER_CLASS,
            nameof(ContentAlignmentOption.Right) => TEXT_ALIGN_RIGHT_CLASS,
            _ => TEXT_ALIGN_LEFT_CLASS
        };

        cssClasses.Add(textAlignmentClass);
        return cssClasses;
    }

    private List<string> GetImageElementCssClasses(ServiceWidgetProperties properties)
    {
        const string IMAGE_BASE_CSS_CLASS = "tg-product_img";
        const string ROUND_CORNERS_TOP_ONLY_CLASS = "top-only";
        List<string> imageLeftRightClasses = ["tg-col", "c-product-img", "object-fit-cover"];

        List<string> cssClasses = [IMAGE_BASE_CSS_CLASS];

        cssClasses.AddRange(GetChildElementCssClasses(properties));

        if (IsImagePositionSide(properties.ImagePosition))
        {
            cssClasses.AddRange(imageLeftRightClasses);
        }

        if (IsImagePositionFullSize(properties.ImagePosition)
        && HasRoundCorners(properties.CornerStyle))
        {
            cssClasses.Add(ROUND_CORNERS_TOP_ONLY_CLASS);
        }

        return cssClasses;
    }

    private List<string> GetChildElementCssClasses(ServiceWidgetProperties properties)
    {
        List<string> cssClasses = [];

        if (!IsImagePositionFullSize(properties.ImagePosition))
        {
            if (properties.DropShadow)
                cssClasses.Add(BS_DROP_SHADOW_CLASS);

            cssClasses.Add(BS_MARGIN_CLASS);
        }
        return cssClasses;
    }

    private bool IsImagePositionFullSize(string imagePosition) =>
        Equals(imagePosition, nameof(ImagePositionOption.FullWidth));

    private bool IsImagePositionSide(string imagePosition) =>
        Equals(imagePosition, nameof(ImagePositionOption.Left)) || Equals(imagePosition, nameof(ImagePositionOption.Right));

    private bool HasRoundCorners(string cornerStyle) =>
        Equals(cornerStyle, nameof(CornerStyleOption.Round)) || Equals(cornerStyle, nameof(CornerStyleOption.VeryRound));
}
