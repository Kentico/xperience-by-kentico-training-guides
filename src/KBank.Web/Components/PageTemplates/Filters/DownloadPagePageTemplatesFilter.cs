using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine.Types.KBank;
using KBank.Web.Components;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace KBank.Web.Components.PageTemplates.Filters;

public class DownloadPagePageTemplatesFilter : IPageTemplateFilter
{
    public IEnumerable<PageTemplateDefinition> Filter(IEnumerable<PageTemplateDefinition> pageTemplates, PageTemplateFilterContext context)
    {
        if (context.PageType.Equals(DownloadPage.CLASS_NAME, StringComparison.InvariantCultureIgnoreCase))
        {
            return pageTemplates.Where(t => GetDownloadPageTemplates().Contains(t.Identifier));
        }
        return pageTemplates.Where(t => !GetDownloadPageTemplates().Contains(t.Identifier));
    }

    public IEnumerable<string> GetDownloadPageTemplates() => new string[] { ComponentIdentifiers.DOWNLOAD_PAGE_TEMPLATE };
}
