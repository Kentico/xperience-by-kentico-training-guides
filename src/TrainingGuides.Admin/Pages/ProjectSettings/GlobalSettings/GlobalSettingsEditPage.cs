using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using TrainingGuides.Admin.ProjectSettings.GlobalSettings;
using TrainingGuides.ProjectSettings;

[assembly: UIPage(
    parentType: typeof(GlobalSettingsEditSection),
    slug: "edit",
    uiPageType: typeof(GlobalSettingsEditPage),
    name: "Edit global settings key",
    templateName: TemplateNames.EDIT,
    order: 0)]

namespace TrainingGuides.Admin.ProjectSettings.GlobalSettings;

public class GlobalSettingsEditPage : InfoEditPage<GlobalSettingsKeyInfo>
{
    [PageParameter(typeof(IntPageModelBinder))]
    public override int ObjectId { get; set; }

    public GlobalSettingsEditPage(IFormComponentMapper formComponentMapper,
        IFormDataBinder formDataBinder)
        : base(formComponentMapper, formDataBinder)
    {
    }

    public override Task ConfigurePage()
    {
        PageConfiguration.UIFormName = "globalsettingskeyedit";
        return base.ConfigurePage();
    }
}