using System;
using System.Collections.Generic;
using System.Linq;

using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace KBank.Web.Components.PageTemplates.Filters;

public class ArticlePagePageTemplatesFilter : IPageTemplateFilter
{
    public IEnumerable<PageTemplateDefinition> Filter(IEnumerable<PageTemplateDefinition> pageTemplates, PageTemplateFilterContext context)
    {
        if (context.ContentTypeName.Equals(ArticlePage.CONTENT_TYPE_NAME, StringComparison.InvariantCultureIgnoreCase))
        {
            return pageTemplates.Where(t => GetHeadingAndSubTemplates().Contains(t.Identifier));
        }
        return pageTemplates.Where(t => !GetHeadingAndSubTemplates().Contains(t.Identifier));
    }

    public IEnumerable<string> GetHeadingAndSubTemplates() => new string[] {ComponentIdentifiers.ARTICLE_PAGE_TEMPLATE };
}