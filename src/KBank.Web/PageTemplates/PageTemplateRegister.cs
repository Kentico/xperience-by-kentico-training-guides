using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using KBank.Web.PageTemplates;

[assembly: RegisterPageTemplate(PageTemplateIdentifiers.HEADING_AND_SUB_TEMPLATE, "Heading and subheading content type template", customViewName: "~/PageTemplates/HeadingAndSub/_HeadingAndSub.cshtml", IconClass = "xp-a-lowercase")]
[assembly: RegisterPageTemplate(PageTemplateIdentifiers.DOWNLOAD_PAGE_TEMPLATE, "Download page content type template", customViewName: "~/PageTemplates/DownloadPage/_DownloadPage.cshtml", IconClass = "xp-arrow-down-line")]
[assembly: RegisterPageTemplate(PageTemplateIdentifiers.HOME_PAGE_TEMPLATE, "Home page content type template", typeof(HomePageTemplateProperties), customViewName: "~/PageTemplates/HomePage/_HomePage.cshtml", IconClass = "xp-market")]
[assembly: RegisterPageTemplate(PageTemplateIdentifiers.SINGLE_ZONE_GENERIC_TEMPLATE, "Single editable zone template", customViewName: "~/PageTemplates/SingleZoneGeneric/_SingleZoneGeneric.cshtml", IconClass = "xp-box")]
