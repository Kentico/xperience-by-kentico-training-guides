using System;
using System.Collections.Generic;
using System.Linq;

using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace KBank.Web.Components.PageTemplates.Filters;

public class DownloadsPagePageTemplatesFilter : IPageTemplateFilter
{
    public IEnumerable<PageTemplateDefinition> Filter(IEnumerable<PageTemplateDefinition> pageTemplates, PageTemplateFilterContext context)
    {
        if (context.ContentTypeName.Equals(DownloadsPage.CONTENT_TYPE_NAME, StringComparison.InvariantCultureIgnoreCase))
        {
            return pageTemplates.Where(t => GetDownloadsPageTemplates().Contains(t.Identifier));
        }
        return pageTemplates.Where(t => !GetDownloadsPageTemplates().Contains(t.Identifier));
    }

    public IEnumerable<string> GetDownloadsPageTemplates() => new string[] { ComponentIdentifiers.DOWNLOADS_PAGE_TEMPLATE };
}
