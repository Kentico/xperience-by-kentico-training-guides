using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;
using Kentico.Xperience.Admin.Base.Forms;

[assembly: UIPage(
    parentType: typeof(GlobalSettingsListingPage),
    slug: "create",
    uiPageType: typeof(GlobalSettingsCreatePage),
    name: "Create global settings key",
    templateName: TemplateNames.EDIT,
    order: 20)]

namespace TrainingGuides.Admin.ProjectSettings;

public class GlobalSettingsCreatePage : CreatePage<GlobalSettingsKeyInfo, GlobalSettingsEdit>
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