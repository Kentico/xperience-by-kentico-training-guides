using KBank.Web.Components;
using KBank.Web.Components.Sections;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

// Sections
[assembly: RegisterSection(ComponentIdentifiers.SINGLE_COLUMN_SECTION, "1 column", typeof(SingleColumnSectionProperties), "~/Components/Sections/SingleColumnSection/_KBank_SingleColumnSection.cshtml", Description = "Single-column section with one full-width zone.", IconClass = "icon-square")]
[assembly: RegisterSection(ComponentIdentifiers.FORM_COLUMN_SECTION, "Form column", typeof(FormColumnSectionProperties), "~/Components/Sections/FormColumnSection/_KBank_FormColumnSection.cshtml", Description = "Form column section.", IconClass = "icon-square")]
[assembly: RegisterSection(ComponentIdentifiers.FORM_COLUMN_SECTION_CONSENT, typeof(FormColumnSectionConsentViewComponent), "Form column: Consent-based", typeof(FormColumnSectionProperties), Description = "Form column section that hides its contents if the visitor has not consented to tracking.", IconClass = "icon-square")]