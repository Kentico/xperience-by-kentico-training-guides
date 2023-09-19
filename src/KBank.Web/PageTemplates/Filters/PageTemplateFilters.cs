using CMS;
using CMS.DataEngine;

using KBank.Web.PageTemplates.Filters;

using Kentico.PageBuilder.Web.Mvc;

[assembly: RegisterModule(typeof(PageTemplateFilters))]

public class PageTemplateFilters : Module
{
    public PageTemplateFilters() : base("PageTemplateFilters")
    {
    }

    protected override void OnInit()
    {
        base.OnInit();

        RegisterPageTemplateFilters();
    }

    private void RegisterPageTemplateFilters()
    {
        PageBuilderFilters.PageTemplates.Add(new HeadingAndSubPageTemplatesFilter());
        PageBuilderFilters.PageTemplates.Add(new DownloadPagePageTemplatesFilter());
    }
}