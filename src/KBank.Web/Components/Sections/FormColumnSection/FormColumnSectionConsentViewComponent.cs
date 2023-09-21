using CMS.ContactManagement;
using CMS.DataProtection;
using CMS.Helpers;
using KBank.Web.Helpers.Cookies;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace KBank.Web.Components.Sections
{
    public class FormColumnSectionConsentViewComponent : ViewComponent
    {
        IConsentAgreementService _consentAgreementService;
        IConsentInfoProvider _consentInfoProvider;

        public FormColumnSectionConsentViewComponent(IConsentAgreementService consentAgreementService, IConsentInfoProvider consentInfoProvider)
        {
            _consentAgreementService = consentAgreementService;
            _consentInfoProvider = consentInfoProvider;
        }

        
        public IViewComponentResult Invoke(ComponentViewModel<FormColumnSectionProperties> sectionProperties)
        {
            //If the CMSCookieLevel is set to 200 (Visitor) or higher, and Data Protection is set up, it means they are of the appropriate consent level for tracking.
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
