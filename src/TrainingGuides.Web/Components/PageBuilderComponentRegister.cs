using TrainingGuides;
using TrainingGuides.Web.Components;
using TrainingGuides.Web.Components.PageTemplates;
using TrainingGuides.Web.Components.Sections;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

// Sections
[assembly: RegisterSection(
    ComponentIdentifiers.SINGLE_COLUMN_SECTION,
    "1 column",
    typeof(SingleColumnSectionProperties),
    "~/Components/Sections/SingleColumnSection/_TrainingGuides_SingleColumnSection.cshtml",
    Description = "Single-column section with one full-width zone.",
    IconClass = "icon-square")]
[assembly: RegisterSection(ComponentIdentifiers.FORM_COLUMN_SECTION,
    "Form column", typeof(FormColumnSectionProperties),
    "~/Components/Sections/FormColumnSection/_TrainingGuides_FormColumnSection.cshtml",
    Description = "Form column section.",
    IconClass = "icon-square")]


//Page templates
[assembly: RegisterPageTemplate(
    identifier: ComponentIdentifiers.ARTICLE_PAGE_TEMPLATE,
    name: "Article page content type template",
    customViewName: "~/Components/PageTemplates/ArticlePage/_ArticlePagePageTemplate.cshtml",
    ContentTypeNames = new string[] { ArticlePage.CONTENT_TYPE_NAME },
    IconClass = "xp-a-lowercase")]
[assembly: RegisterPageTemplate(
    identifier: ComponentIdentifiers.DOWNLOADS_PAGE_TEMPLATE,
    name: "Downloads page content type template",
    customViewName: "~/Components/PageTemplates/DownloadsPage/_DownloadsPagePageTemplate.cshtml",
    ContentTypeNames = new string[] { DownloadsPage.CONTENT_TYPE_NAME },
    IconClass = "xp-arrow-down-line")]
[assembly: RegisterPageTemplate(
    identifier: ComponentIdentifiers.LANDING_PAGE_TEMPLATE,
    name: "Landing page content type template",
    propertiesType: typeof(LandingPageTemplateProperties),
    customViewName: "~/Components/PageTemplates/LandingPage/_LandingPagePageTemplate.cshtml",
    ContentTypeNames = new string[] { LandingPage.CONTENT_TYPE_NAME },
    IconClass = "xp-market")]
[assembly: RegisterPageTemplate(
    identifier: ComponentIdentifiers.EMPTY_PAGE_TEMPLATE,
    name: "Empty page content type template",
    customViewName: "~/Components/PageTemplates/EmptyPage/_EmptyPagePageTemplate.cshtml",
    IconClass = "xp-binder")]
