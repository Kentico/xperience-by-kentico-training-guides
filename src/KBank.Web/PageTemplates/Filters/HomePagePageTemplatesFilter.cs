using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine.Types.KBank;

using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace KBank.Web.PageTemplates.Filters;

public class HomePagePageTemplatesFilter : IPageTemplateFilter
{
    public IEnumerable<PageTemplateDefinition> Filter(IEnumerable<PageTemplateDefinition> pageTemplates, PageTemplateFilterContext context)
    {
        if (context.PageType.Equals(HomePage.CLASS_NAME, StringComparison.InvariantCultureIgnoreCase))
        {
            return pageTemplates.Where(t => GetHomePageTemplates().Contains(t.Identifier));
        }
        return pageTemplates.Where(t => !GetHomePageTemplates().Contains(t.Identifier));
    }

    public IEnumerable<string> GetHomePageTemplates() => new string[] { PageTemplateIdentifiers.HOME_PAGE_TEMPLATE };
}
