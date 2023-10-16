using KBank.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace KBank.Web.Components.PageTemplates;

public class DownloadsPageViewModel
{
    public string Heading { get; set; }
    public IEnumerable<AssetViewModel> Assets { get; set; }

    public static DownloadsPageViewModel GetViewModel(DownloadsPage downloadsPage)
    {
        if (downloadsPage == null)
        {
            return new DownloadsPageViewModel();
        }
        return new DownloadsPageViewModel
        {
            Heading = downloadsPage.DownloadsPageContent.FirstOrDefault()?.Heading,
            Assets = downloadsPage.DownloadsPageContent.FirstOrDefault()?.Assets?.Select(asset => AssetViewModel.GetViewModel(asset as Asset))
        };
    }
}

