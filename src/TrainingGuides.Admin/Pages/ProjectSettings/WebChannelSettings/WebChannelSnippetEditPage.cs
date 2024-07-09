using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;
using Kentico.Xperience.Admin.Base.Forms;

[assembly: UIPage(
    parentType: typeof(WebChannelSnippetEditSection),
    slug: "edit",
    uiPageType: typeof(WebChannelSnippetEditPage),
    name: "Edit snippet",
    templateName: TemplateNames.EDIT,
    order: 0)]

namespace TrainingGuides.Admin.ProjectSettings;

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