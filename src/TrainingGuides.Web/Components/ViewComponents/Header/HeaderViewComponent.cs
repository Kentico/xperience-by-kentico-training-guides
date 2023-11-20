using CMS.ContentEngine;
using CMS.Helpers;
using TrainingGuides.Web.Models;
using TrainingGuides.Web.Services.Content;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace TrainingGuides.Web.Components.ViewComponents.Header
{
    public class HeaderViewComponent : ViewComponent
    {

        private readonly IProgressiveCache _progressiveCache;
        private readonly IContentQueryExecutor contentQueryExecutor;
        private readonly IContentQueryResultMapper contentQueryResultMapper;
        private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
        private readonly IContentItemRetrieverService<Asset> contentItemRetriever;

        public HeaderViewComponent(
            IProgressiveCache progressiveCache,
            IContentQueryExecutor contentQueryExecutor,
            IContentQueryResultMapper contentQueryResultMapper,
            IPreferredLanguageRetriever preferredLanguageRetriever,
            IContentItemRetrieverService<Asset> contentItemRetriever)
        {
            _progressiveCache = progressiveCache;
            this.contentQueryExecutor = contentQueryExecutor;
            this.contentQueryResultMapper = contentQueryResultMapper;
            this.preferredLanguageRetriever = preferredLanguageRetriever;
            this.contentItemRetriever = contentItemRetriever;
        }

        public async Task<IViewComponentResult> InvokeAsync()

        {
            var model = new HeaderViewModel()
            {
                Heading = "Training guides"
            };
            return View("~/Components/ViewComponents/Header/_Header.cshtml", model);
        }
    }
}
