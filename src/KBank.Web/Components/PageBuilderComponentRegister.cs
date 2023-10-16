using KBank.Web.Components;
using KBank.Web.Components.Sections;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using KBank.Web.Components.PageTemplates;

// Sections
[assembly: RegisterSection(ComponentIdentifiers.SINGLE_COLUMN_SECTION, "1 column", typeof(SingleColumnSectionProperties), "~/Components/Sections/SingleColumnSection/_KBank_SingleColumnSection.cshtml", Description = "Single-column section with one full-width zone.", IconClass = "icon-square")]
[assembly: RegisterSection(ComponentIdentifiers.FORM_COLUMN_SECTION, "Form column", typeof(FormColumnSectionProperties), "~/Components/Sections/FormColumnSection/_KBank_FormColumnSection.cshtml", Description = "Form column section.", IconClass = "icon-square")]


//Page templates
[assembly: RegisterPageTemplate(ComponentIdentifiers.HEADING_AND_SUB_TEMPLATE, "Heading and subheading content type template", customViewName: "~/Components/PageTemplates/HeadingAndSub/_HeadingAndSub.cshtml", IconClass = "xp-a-lowercase")]
[assembly: RegisterPageTemplate(ComponentIdentifiers.DOWNLOAD_PAGE_TEMPLATE, "Download page content type template", customViewName: "~/Components/PageTemplates/DownloadPage/_DownloadPage.cshtml", IconClass = "xp-arrow-down-line")]
[assembly: RegisterPageTemplate(ComponentIdentifiers.HOME_PAGE_TEMPLATE, "Home page content type template", typeof(HomePageTemplateProperties), customViewName: "~/Components/PageTemplates/HomePage/_HomePage.cshtml", IconClass = "xp-market")]
[assembly: RegisterPageTemplate(ComponentIdentifiers.SINGLE_ZONE_GENERIC_TEMPLATE, "Single editable zone template", customViewName: "~/Components/PageTemplates/SingleZoneGeneric/_SingleZoneGeneric.cshtml", IconClass = "xp-box")]
