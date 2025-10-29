﻿using CMS.DataEngine;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.LandingPages.Widgets.HeroBanner;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(
    identifier: HeroBannerWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(HeroBannerWidgetViewComponent),
    name: "Hero banner",
    propertiesType: typeof(HeroBannerWidgetProperties),
    Description = "Displays text, image, and benefits.",
    IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.LandingPages.Widgets.HeroBanner;

public class HeroBannerWidgetViewComponent : ViewComponent
{
    private readonly IContentItemRetrieverService contentItemRetrieverService;
    private readonly IEnumStringService enumStringService;
    private readonly IContentTypeService contentTypeService;

    public const string IDENTIFIER = "TrainingGuides.HeroBanner";

    public HeroBannerWidgetViewComponent(
        IContentItemRetrieverService contentItemRetrieverService,
        IEnumStringService enumStringService,
        IContentTypeService contentTypeService)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.enumStringService = enumStringService;
        this.contentTypeService = contentTypeService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(HeroBannerWidgetProperties properties)
    {
        var banner = new HeroBannerWidgetViewModel();

        if (string.Equals(properties.Mode, "currentProductPage"))
        {
            var productPage = await contentItemRetrieverService.RetrieveCurrentPage<ProductPage>(3);

            int? productClassId = contentTypeService.GetContentTypeId(ProductPage.CONTENT_TYPE_NAME);

            if (productPage is not null && productPage.SystemFields.ContentItemContentTypeID == productClassId)
            {
                banner = GetProductPageBanner(productPage);

                if (banner is not null)
                {
                    banner.CTALink = properties.ProductPageAnchor;
                    banner.CTAText = properties.CTA;
                    banner.LinkTitle = properties.CTA;
                }
            }
        }
        else if (string.Equals(properties.Mode, "productPage"))
        {
            if (properties.ProductPage.FirstOrDefault() is not null)
            {
                var productPageGuid = properties.ProductPage?.Select(i => i.Identifier).FirstOrDefault();
                var productPage = productPageGuid.HasValue
                    ? await contentItemRetrieverService
                        .RetrieveWebPageByContentItemGuid<ProductPage>((Guid)productPageGuid, 3)
                    : null;

                banner = GetProductPageBanner(productPage);
                if (banner is not null)
                {
                    string relativeUrl = productPage?.SystemFields.WebPageUrlPath is not null
                        ? $"~/{productPage.SystemFields.WebPageUrlPath}"
                        : string.Empty;
                    banner.CTALink = relativeUrl + (string.IsNullOrWhiteSpace(properties.SelectedProductPageAnchor)
                                        ? string.Empty
                                        : $"#{properties.SelectedProductPageAnchor}");
                    banner.CTAText = properties.CTA;
                }
            }
        }
        else
        {
            if (properties.Hero is not null && properties.Hero.Any())
            {
                var heroGuid = properties?.Hero?.Select(i => i.Identifier).ToList().FirstOrDefault();

                var hero = heroGuid.HasValue
                    ? await contentItemRetrieverService
                        .RetrieveContentItemByGuid<Hero>((Guid)heroGuid, 3)
                    : null;

                banner = GetModel(hero, properties);

                if (banner?.Link is not null)
                {
                    banner.CTALink = !string.IsNullOrEmpty(banner.Link.LinkUrl) ? banner!.Link.LinkUrl : banner.Link.LinkToExternal ?? string.Empty;
                    banner.CTAText = !string.IsNullOrEmpty(properties?.CTA) ? properties.CTA : banner.Link.CallToAction;
                    banner.LinkTitle = banner.Link.LinkTitleText;
                }
            }
        }

        if (banner is not null)
        {

            banner.DisplayCTA = !string.IsNullOrEmpty(banner.CTALink) && !string.IsNullOrEmpty(banner.CTAText) && properties!.DisplayCTA;
            banner.OpenInNewTab = properties!.OpenInNewTab;

            if (properties.ChangeDesign)
            {
                banner.ShowBenefits = properties.ShowBenefits;
                banner.FullWidth = (properties.Width ?? "circle").Equals("full", StringComparison.InvariantCultureIgnoreCase);
                banner.TextColor = properties.TextColor;
                banner.ThemeClass = enumStringService.Parse(properties.TextColor, TextColorOption.Dark) switch
                {
                    TextColorOption.Light => "light",
                    TextColorOption.Dark => "",
                    _ => ""
                };
                banner.ShowImage = properties.ShowImage;
            }

            if (!string.IsNullOrEmpty(banner.Media?.FilePath))
            {
                string backgroundImageStyle = $"background-image: url('{Url.Content(banner.Media.FilePath)}');";
                string backgroundNoRepeatStyle = "background-repeat: no-repeat;background-size: cover;background-position: center";

                string backgroundStyle = banner.FullWidth
                        ? backgroundImageStyle
                        : $"{backgroundImageStyle};{backgroundNoRepeatStyle}";

                banner.StyleAttributeHtml = new HtmlString($"style=\"{backgroundStyle}\"");
            }
        }

        return View("~/Features/LandingPages/Widgets/HeroBanner/HeroBannerWidget.cshtml", banner);
    }

    private HeroBannerWidgetViewModel? GetProductPageBanner(ProductPage? productPage) =>
        productPage is null ? null : GetHeroBannerViewModel(productPage);

    private static HeroBannerWidgetViewModel? GetHeroBannerViewModel(ProductPage productPage)
    {
        var product = productPage.ProductPageProduct.FirstOrDefault();

        if (product is not null)
        {
            var benefits = product.ProductBenefits.ToList();
            var media = product.ProductMedia.FirstOrDefault();

            return new HeroBannerWidgetViewModel()
            {
                Header = product.ProductName,
                SubheaderHtml = new HtmlString(product.ProductShortDescription),
                Benefits = benefits.Select(BenefitViewModel.GetViewModel).ToList(),
                Media = media is not null
                    ? AssetViewModel.GetViewModel(media)
                    : new AssetViewModel()
            };
        }

        return null;
    }

    private HeroBannerWidgetViewModel? GetModel(Hero? hero, HeroBannerWidgetProperties? properties)
    {
        if (hero is null || properties is null)
        {
            return null;
        }

        var url = hero.HeroTarget?.FirstOrDefault()?.GetUrl();

        var media = hero.HeroMedia.FirstOrDefault();

        var model = new HeroBannerWidgetViewModel()
        {
            Header = hero.HeroHeader,
            SubheaderHtml = new HtmlString(hero.HeroSubheader),
            Benefits = hero.HeroBenefits.Select(BenefitViewModel.GetViewModel).ToList(),
            Link = new LinkViewModel()
            {
                LinkUrl = url?.RelativePath ?? string.Empty,
                CallToAction = hero.HeroCallToAction
            },
            Media = media is not null
                ? AssetViewModel.GetViewModel(media)
                : new AssetViewModel()
        };

        return model;
    }
}