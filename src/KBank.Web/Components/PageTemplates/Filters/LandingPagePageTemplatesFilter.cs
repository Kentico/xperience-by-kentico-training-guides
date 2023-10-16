using System;
using System.Collections.Generic;
using System.Linq;


//using KBank.Web.Components;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace KBank.Web.Components.PageTemplates.Filters;

public class LandingPagePageTemplatesFilter : IPageTemplateFilter
{
    public IEnumerable<PageTemplateDefinition> Filter(IEnumerable<PageTemplateDefinition> pageTemplates, PageTemplateFilterContext context)
    {
        if (context.ContentTypeName.Equals(LandingPage.CONTENT_TYPE_NAME, StringComparison.InvariantCultureIgnoreCase))
        {
            return pageTemplates.Where(t => GetHomePageTemplates().Contains(t.Identifier));
        }
        return pageTemplates.Where(t => !GetHomePageTemplates().Contains(t.Identifier));
    }

    public IEnumerable<string> GetHomePageTemplates() => new string[] { ComponentIdentifiers.LANDING_PAGE_TEMPLATE };
}
