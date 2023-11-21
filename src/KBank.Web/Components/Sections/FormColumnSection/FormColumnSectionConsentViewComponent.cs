using TrainingGuides.Web.Helpers.Cookies;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.Components.Sections
{
    public class FormColumnSectionConsentViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ComponentViewModel<FormColumnSectionProperties> sectionProperties)
        {
            //If the CMSCookieLevel is set to 200 (Visitor) or higher, and Data Protection is set up, it means the visitor has the appropriate consent level for tracking.
            bool showContents = CookieConsentHelper.CurrentContactIsVisitorOrHigher();

            var model = new FormColumnSectionConsentViewModel()
            {
                SectionAnchor = sectionProperties.Properties.SectionAnchor,
                ShowContents = showContents
            };

            return View("~/Components/Sections/FormColumnSection/_KBank_FormColumnSectionConsent.cshtml", model);
        }
    }
}