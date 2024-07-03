using CMS.DataEngine;
using CMS.Websites.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.ProjectSettings;

namespace TrainingGuides.Web.Features.CodeSnippets;

public class CodeSnippetsViewComponent : ViewComponent
{
    private readonly IInfoProvider<WebChannelSnippetInfo> webChannelSnippetInfoProvider;
    private readonly IInfoProvider<WebChannelSettingsInfo> webChannelSettingsInfoProvider;
    private readonly IWebsiteChannelContext websiteChannelContext;

    public CodeSnippetsViewComponent(IInfoProvider<WebChannelSnippetInfo> webChannelSnippetInfoProvider, IInfoProvider<WebChannelSettingsInfo> webChannelSettingsInfoProvider, IWebsiteChannelContext websiteChannelContext)
    {
        this.webChannelSnippetInfoProvider = webChannelSnippetInfoProvider;
        this.webChannelSettingsInfoProvider = webChannelSettingsInfoProvider;
        this.websiteChannelContext = websiteChannelContext;
    }

    public async Task<IViewComponentResult> InvokeAsync(CodeSnippetType codeSnippetType, bool addLabelComments = false)
    {
        int currentChannelID = websiteChannelContext.WebsiteChannelID;

        var settings = await webChannelSettingsInfoProvider
            .Get()
            .WhereEquals(nameof(WebChannelSettingsInfo.WebChannelSettingsChannelID), currentChannelID)
            .GetEnumerableTypedResultAsync();

        var setting = settings.FirstOrDefault();

        var snippets = await webChannelSnippetInfoProvider
            .Get()
            .WhereEquals(nameof(WebChannelSnippetInfo.WebChannelSnippetWebChannelSettingsID), setting?.WebChannelSettingsID ?? 0)
            .WhereEquals(nameof(WebChannelSnippetInfo.WebChannelSnippetType), codeSnippetType.ToString())
            .GetEnumerableTypedResultAsync();

        var model = snippets.Select(snippet => new CodeSnippetViewModel
        {
            CodeSnippet = new HtmlString(snippet.WebChannelSnippetCode),
            CodeSnippetType = snippet.WebChannelSnippetType,
            CodeSnippetLabel = addLabelComments
            ? snippet.WebChannelSnippetDisplayName
            : string.Empty
        });
        return View("~/Features/CodeSnippets/CodeSnippetsViewComponent.cshtml", model);
    }
}
