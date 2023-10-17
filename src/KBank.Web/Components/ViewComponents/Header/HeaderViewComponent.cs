using CMS.ContentEngine;
using CMS.Helpers;
using KBank.Web.Models;
using KBank.Web.Services;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KBank.Web.Components.ViewComponents.Header
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
                Logo = await GetLogo()
            };
            return View("~/Components/ViewComponents/Header/_Header.cshtml", model);
        }


        private async Task<AssetViewModel> GetLogo()
        {
            Asset asset = await _progressiveCache.Load(async cs => await LoadLogo(cs), new CacheSettings(1200, "headerlogo|main"));

            ContentItemAsset file = asset?.File;

            if (file == null) return null;

            return new AssetViewModel()
            {
                FilePath = file.Url,
                AltText = asset.AltText,
                UseInternalOnly = asset.UseInternalOnly,
                Description = asset.Description
            };
        }


        private async Task<Asset> LoadLogo(CacheSettings cs)
        {
            const string KBANK_LOGO_DESCRIPTION = "KBankLogo";

            var asset = await contentItemRetriever.RetrieveContentItem(
                Asset.CONTENT_TYPE_NAME,
                config => config
                    .Where(where => where.WhereEquals(nameof(Asset.Description), KBANK_LOGO_DESCRIPTION))
                    .TopN(1),
                container => contentQueryResultMapper.Map<Asset>(container));

            if (asset == null) return null;

            cs.CacheDependency = CacheHelper.GetCacheDependency($"contentitem|byid|{asset?.SystemFields.ContentItemID}");

            return asset;
        }
    }
}