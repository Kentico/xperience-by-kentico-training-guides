using KBank.Web.Components;
using KBank.Web.Components.Sections;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

// Sections
[assembly: RegisterSection(ComponentIdentifiers.SINGLE_COLUMN_SECTION, "1 column", typeof(SingleColumnSectionProperties), "~/Components/Sections/_Kentico_KBank_SingleColumnSection.cshtml", Description = "Single-column section with one full-width zone.", IconClass = "icon-square")]
[assembly: RegisterSection(ComponentIdentifiers.FORM_COLUMN_SECTION, "Form column", typeof(FormColumnSectionProperties), "~/Components/Sections/_Kentico_KBank_FormColumnSection.cshtml", Description = "Form column section.", IconClass = "icon-square")]