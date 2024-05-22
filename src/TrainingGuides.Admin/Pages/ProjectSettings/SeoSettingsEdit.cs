using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;
using Kentico.Xperience.Admin.Base.Forms;
using CMS.DataEngine;

// Registers the UI page
[assembly: UIPage(
    parentType: typeof(WebChannelSettingsEditSection),
    slug: "edit",
    uiPageType: typeof(SeoSettingsEdit),
    name: "SEO settings",
    templateName: TemplateNames.EDIT,
    order: 0)]

namespace TrainingGuides.Admin.ProjectSettings;

public class SeoSettingsEdit : InfoEditPage<SeoSettingsInfo>
{
    private readonly IInfoProvider<SeoSettingsInfo> seoSettingsInfoProvider;

    [PageParameter(typeof(IntPageModelBinder))]
    public int WebChannelSettingsId { get; set; }

    // Retrieves the SEO settings ID based on the WebChannelSettingsId.
    // The UI will try to set the ObjectID to value of the parent WebChannelSettingsId, so we have to ignore that.
    [PageParameter(typeof(IntPageModelBinder))]
    public override int ObjectId
    {
        get => seoSettingsInfoProvider.Get()
                .WhereEquals(nameof(SeoSettingsInfo.SeoSettingsWebChannelSettingId), WebChannelSettingsId)
                .FirstOrDefault()?
                .SeoSettingsID ?? 0;
        set { }
    }

    public SeoSettingsEdit(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder, IInfoProvider<SeoSettingsInfo> seoSettingsInfoProvider)
             : base(formComponentMapper, formDataBinder)
    {
        this.seoSettingsInfoProvider = seoSettingsInfoProvider;
    }

    public override Task ConfigurePage()
    {
        PageConfiguration.UIFormName = "seosettingsedit";
        return base.ConfigurePage();
    }
}