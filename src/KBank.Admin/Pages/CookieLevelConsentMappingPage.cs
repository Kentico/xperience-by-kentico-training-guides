using KBank.Admin.Pages;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Admin.DigitalMarketing.UIPages;

[assembly: UIPage(typeof(DataProtectionApplication), "cookielevelconsentmapping", typeof(CookieLevelConsentMappingPage), "Cookie level consent mapping",
    TemplateNames.EDIT, UIPageOrder.First + 1)]
namespace KBank.Admin.Pages
{
    internal class CookieLevelConsentMappingPage : InfoEditPage<CookieLevelConsentMappingInfo>
    {
        private readonly ICookieLevelConsentMappingInfoProvider _cookieLevelConsentMappingInfoProvider;

        public CookieLevelConsentMappingPage(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder,
        ICookieLevelConsentMappingInfoProvider generalSettingsInfoProvider) : base(formComponentMapper, formDataBinder)
        {
            _cookieLevelConsentMappingInfoProvider = generalSettingsInfoProvider;
        }


        public override int ObjectId
        {
            get
            {
                var mappings = GetOrCreateMappings();
                return mappings.CookieLevelConsentMappingID;
            }
            set => throw new InvalidOperationException("The $ObjectId value cannot be set.");
        }


        public override Task ConfigurePage()
        {
            PageConfiguration.UIFormName = "CookieLevelConsentMapping";

            return base.ConfigurePage();
        }
        

        private CookieLevelConsentMappingInfo GetOrCreateMappings()
        {
            var item = _cookieLevelConsentMappingInfoProvider.Get()?.FirstOrDefault();

            if (item == null)
            {
                item = new CookieLevelConsentMappingInfo();
                item.CookieLevelConsentMappingGuid = Guid.NewGuid();
                _cookieLevelConsentMappingInfoProvider.Set(item);
            }

            return item;
        }
    }
}