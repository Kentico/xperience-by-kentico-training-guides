using Kentico.Xperience.Admin.Base;
using TrainingGuides.ProjectSettings;
using TrainingGuides.Admin.ProjectSettings;
using Kentico.Xperience.Admin.Base.Forms;

// Registers the UI page
[assembly: UIPage(
    parentType: typeof(WebChannelSnippetList),
    slug: "create",
    uiPageType: typeof(WebChannelSnippetCreate),
    name: "Create snippet",
    templateName: TemplateNames.EDIT,
    order: 20)]

namespace TrainingGuides.Admin.ProjectSettings;

public class WebChannelSnippetCreate : CreatePage<WebChannelSnippetInfo, WebChannelSnippetEditSection>
{
    [PageParameter(typeof(IntPageModelBinder), typeof(WebChannelSettingsEditSection))]
    public int WebChannelSettingsId { get; set; }

    public WebChannelSnippetCreate(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder, IPageUrlGenerator pageUrlGenerator)
        : base(formComponentMapper, formDataBinder, pageUrlGenerator)
    {
    }

    public override Task ConfigurePage()
    {
        AdditionalUrlParameters.Add(WebChannelSettingsId.ToString());
        PageConfiguration.UIFormName = "webchannelsnippetedit";
        return base.ConfigurePage();
    }

    protected override Task FinalizeInfoObject(WebChannelSnippetInfo infoObject, IFormFieldValueProvider fieldValueProvider, CancellationToken cancellationToken)
    {
        infoObject.WebChannelSnippetWebChannelSettingsId = WebChannelSettingsId;

        return base.FinalizeInfoObject(infoObject, fieldValueProvider, cancellationToken);
    }
}