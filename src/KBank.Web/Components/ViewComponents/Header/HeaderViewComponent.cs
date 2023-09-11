using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.KBank;

using Kentico.Content.Web.Mvc;
using KBank.Web.Models;

using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using System.Linq;
using CMS.Helpers;

namespace KBank.Web.Components.ViewComponents.Header
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IPageRetriever _pageRetriever;
        private readonly IProgressiveCache _progressiveCache;

        public HeaderViewComponent(IPageRetriever pageRetriever, IProgressiveCache progressiveCache)
        {
            _pageRetriever = pageRetriever;
            _progressiveCache = progressiveCache;
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
            Asset asset = await _progressiveCache.Load(async cs => await LoadLogo(cs), new CacheSettings(720, "headerlogo|main"));

            if (asset == null) return null;

            ContentItemAsset file = asset.Fields?.File;

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
            var assetList = await _pageRetriever.RetrieveAsync<Asset>(query => query
                .WhereEquals("Description", "KBankLogo")
                .WhereEquals("UseInternalOnly", 0)
                .TopN(1));  

            var asset = assetList.FirstOrDefault();
            cs.CacheDependency = CacheHelper.GetCacheDependency($"kentico.asset|byid|{asset?.AssetID}");

            return asset;
        }
    }
}