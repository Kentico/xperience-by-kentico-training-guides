using KBank.Web.Components;
using KBank.Web.Components.Sections;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using KBank.Web.Components.PageTemplates;

// Sections
[assembly: RegisterSection(ComponentIdentifiers.SINGLE_COLUMN_SECTION, "1 column", typeof(SingleColumnSectionProperties), "~/Components/Sections/SingleColumnSection/_KBank_SingleColumnSection.cshtml", Description = "Single-column section with one full-width zone.", IconClass = "icon-square")]
[assembly: RegisterSection(ComponentIdentifiers.FORM_COLUMN_SECTION, "Form column", typeof(FormColumnSectionProperties), "~/Components/Sections/FormColumnSection/_KBank_FormColumnSection.cshtml", Description = "Form column section.", IconClass = "icon-square")]


//Page templates
[assembly: RegisterPageTemplate(ComponentIdentifiers.ARTICLE_PAGE_TEMPLATE, "Article page content type template", customViewName: "~/Components/PageTemplates/ArticlePage/_ArticlePage.cshtml", IconClass = "xp-a-lowercase")]
[assembly: RegisterPageTemplate(ComponentIdentifiers.DOWNLOADS_PAGE_TEMPLATE, "Downloads page content type template", customViewName: "~/Components/PageTemplates/DownloadsPage/_DownloadsPage.cshtml", IconClass = "xp-arrow-down-line")]
[assembly: RegisterPageTemplate(ComponentIdentifiers.LANDING_PAGE_TEMPLATE, "Landing page content type template", typeof(LandingPageTemplateProperties), customViewName: "~/Components/PageTemplates/LandingPage/_LandingPage.cshtml", IconClass = "xp-market")]

