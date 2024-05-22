using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;
using Kentico.Xperience.Admin.Base.Forms;

// Registers the UI page
[assembly: UIPage(
    parentType: typeof(GlobalSettingsList),
    slug: "create",
    uiPageType: typeof(GlobalSettingsCreate),
    name: "Create global settings key",
    templateName: TemplateNames.EDIT,
    order: 20)]

namespace TrainingGuides.Admin.ProjectSettings;

public class GlobalSettingsCreate : CreatePage<GlobalSettingsKeyInfo, GlobalSettingsEdit>
{
    public GlobalSettingsCreate(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder, IPageUrlGenerator pageUrlGenerator)
        : base(formComponentMapper, formDataBinder, pageUrlGenerator)
    {
    }

    public override Task ConfigurePage()
    {
        PageConfiguration.UIFormName = "globalsettingskeyedit";
        return base.ConfigurePage();
    }
}