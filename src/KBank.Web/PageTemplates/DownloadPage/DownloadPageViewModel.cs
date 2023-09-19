using CMS.DocumentEngine.Types.KBank;
using KBank.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace KBank.Web.PageTemplates;

public class DownloadPageViewModel
{
    public string Heading { get; set; }
    public IEnumerable<AssetViewModel> Assets { get; set; }

    public static DownloadPageViewModel GetViewModel(DownloadPage downloadPage)
    {
        return new DownloadPageViewModel
        {
            Heading = downloadPage.Heading,
            Assets = downloadPage.Fields.Assets.Select(asset => AssetViewModel.GetViewModel(asset as Asset))
        };
    }
}

