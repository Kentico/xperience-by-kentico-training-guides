using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using TrainingGuides.Admin.ProjectSettings.WebChannelSettings;
using TrainingGuides.ProjectSettings;

[assembly: UIPage(
    parentType: typeof(WebChannelSnippetListingPage),
    slug: "create",
    uiPageType: typeof(WebChannelSnippetCreatePage),
    name: "Create snippet",
    templateName: TemplateNames.EDIT,
    order: 20)]

namespace TrainingGuides.Admin.ProjectSettings.WebChannelSettings;

public class WebChannelSnippetCreatePage : CreatePage<WebChannelSnippetInfo, WebChannelSnippetEditSection>
{
    [PageParameter(typeof(IntPageModelBinder), typeof(WebChannelSettingsEditSection))]
    public int WebChannelSettingsId { get; set; }

    public WebChannelSnippetCreatePage(IFormComponentMapper formComponentMapper,
        IFormDataBinder formDataBinder,
        IPageUrlGenerator pageUrlGenerator)
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
        infoObject.WebChannelSnippetWebChannelSettingsID = WebChannelSettingsId;

        return base.FinalizeInfoObject(infoObject, fieldValueProvider, cancellationToken);
    }
}