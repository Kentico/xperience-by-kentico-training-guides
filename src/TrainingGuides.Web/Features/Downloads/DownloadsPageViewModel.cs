﻿using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Downloads;

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
            Heading = downloadsPage.DownloadsPageContent.FirstOrDefault()?.DownloadsHeading,
            Assets = downloadsPage.DownloadsPageContent.FirstOrDefault()?.DownloadsAssets?.Select(asset => AssetViewModel.GetViewModel(asset))
        };
    }
}