using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine.Types.KBank;

using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace KBank.Web.PageTemplates.Filters;

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

    public IEnumerable<string> GetDownloadPageTemplates() => new string[] { PageTemplateIdentifiers.DOWNLOAD_PAGE_TEMPLATE };
}
