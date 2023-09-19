using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine.Types.KBank;

using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace KBank.Web.PageTemplates.Filters;

public class HeadingAndSubPageTemplatesFilter : IPageTemplateFilter
{
    public IEnumerable<PageTemplateDefinition> Filter(IEnumerable<PageTemplateDefinition> pageTemplates, PageTemplateFilterContext context)
    {
        if (context.PageType.Equals(HeadingAndSub.CLASS_NAME, StringComparison.InvariantCultureIgnoreCase))
        {
            return pageTemplates.Where(t => GetHeadingAndSubTemplates().Contains(t.Identifier));
        }
        return pageTemplates.Where(t => !GetHeadingAndSubTemplates().Contains(t.Identifier));
    }

    public IEnumerable<string> GetHeadingAndSubTemplates() => new string[] { PageTemplateIdentifiers.HEADING_AND_SUB_TEMPLATE };
}