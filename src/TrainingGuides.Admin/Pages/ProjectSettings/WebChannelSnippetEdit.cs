using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;
using Kentico.Xperience.Admin.Base.Forms;

// Registers the UI page
[assembly: UIPage(
    parentType: typeof(WebChannelSnippetEditSection),
    slug: "edit",
    uiPageType: typeof(WebChannelSnippetEdit),
    name: "Edit snippet",
    templateName: TemplateNames.EDIT,
    order: 0)]

namespace TrainingGuides.Admin.ProjectSettings;

public class WebChannelSnippetEdit : InfoEditPage<WebChannelSnippetInfo>
{
    [PageParameter(typeof(IntPageModelBinder), typeof(WebChannelSnippetEditSection))]
    public override int ObjectId { get; set; }

    [PageParameter(typeof(IntPageModelBinder), typeof(WebChannelSnippetEditSection))]
    public int WebChannelSettingsId { get; set; }

    public WebChannelSnippetEdit(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder)
             : base(formComponentMapper, formDataBinder)
    {
    }

    public override Task ConfigurePage()
    {
        PageConfiguration.UIFormName = "webchannelsnippetedit";
        return base.ConfigurePage();
    }
}