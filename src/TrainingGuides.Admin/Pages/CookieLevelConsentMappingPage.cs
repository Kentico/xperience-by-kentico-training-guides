using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Admin.DigitalMarketing.UIPages;
using TrainingGuides.Admin.Pages;
using TrainingGuides.DataProtectionCustomizations;

[assembly: UIPage(
    parentType: typeof(DataProtectionContentLanguage),
    slug: "cookie-level-consent-mapping",
    uiPageType: typeof(CookieLevelConsentMappingPage),
    name: "Cookie level consent mapping",
    templateName: TemplateNames.EDIT,
    order: UIPageOrder.First + 1)]

namespace TrainingGuides.Admin.Pages;

internal class CookieLevelConsentMappingPage : InfoEditPage<CookieLevelConsentMappingInfo>
{
    private readonly ICookieLevelConsentMappingInfoProvider cookieLevelConsentMappingInfoProvider;

    public CookieLevelConsentMappingPage(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder,
    ICookieLevelConsentMappingInfoProvider generalSettingsInfoProvider) : base(formComponentMapper, formDataBinder)
    {
        cookieLevelConsentMappingInfoProvider = generalSettingsInfoProvider;
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
        PageConfiguration.UIFormName = "cookielevelconsentmapping";

        return base.ConfigurePage();
    }


    private CookieLevelConsentMappingInfo GetOrCreateMappings()
    {
        var item = cookieLevelConsentMappingInfoProvider.Get()?.FirstOrDefault();

        if (item == null)
        {
            item = new CookieLevelConsentMappingInfo
            {
                CookieLevelConsentMappingGuid = Guid.NewGuid()
            };
            cookieLevelConsentMappingInfoProvider.Set(item);
        }

        return item;
    }
}
