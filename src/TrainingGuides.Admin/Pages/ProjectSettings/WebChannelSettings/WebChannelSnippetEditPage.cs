using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using TrainingGuides.Admin.ProjectSettings.WebChannelSettings;
using TrainingGuides.ProjectSettings;

[assembly: UIPage(
    parentType: typeof(WebChannelSnippetEditSection),
    slug: "edit",
    uiPageType: typeof(WebChannelSnippetEditPage),
    name: "Edit snippet",
    templateName: TemplateNames.EDIT,
    order: 0)]

namespace TrainingGuides.Admin.ProjectSettings.WebChannelSettings;

public class WebChannelSnippetEditPage : InfoEditPage<WebChannelSnippetInfo>
{
    [PageParameter(typeof(IntPageModelBinder), typeof(WebChannelSnippetEditSection))]
    public override int ObjectId { get; set; }

    public WebChannelSnippetEditPage(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder)
             : base(formComponentMapper, formDataBinder)
    {
    }

    public override Task ConfigurePage()
    {
        PageConfiguration.UIFormName = "webchannelsnippetedit";
        return base.ConfigurePage();
    }
}