using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using TrainingGuides.Admin.ProjectSettings.GlobalSettings;
using TrainingGuides.ProjectSettings;

[assembly: UIPage(
    parentType: typeof(GlobalSettingsListingPage),
    slug: "create",
    uiPageType: typeof(GlobalSettingsCreatePage),
    name: "Create global settings key",
    templateName: TemplateNames.EDIT,
    order: 20)]

namespace TrainingGuides.Admin.ProjectSettings.GlobalSettings;

public class GlobalSettingsCreatePage : CreatePage<GlobalSettingsKeyInfo, GlobalSettingsEditPage>
{
    public GlobalSettingsCreatePage(IFormComponentMapper formComponentMapper,
        IFormDataBinder formDataBinder,
        IPageUrlGenerator pageUrlGenerator)
        : base(formComponentMapper, formDataBinder, pageUrlGenerator)
    {
    }

    public override Task ConfigurePage()
    {
        PageConfiguration.UIFormName = "globalsettingskeyedit";
        return base.ConfigurePage();
    }
}