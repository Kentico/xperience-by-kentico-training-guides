﻿using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Downloads;

public class DownloadsPageViewModel
{
    public string Heading { get; set; } = string.Empty;
    public IEnumerable<AssetViewModel> Assets { get; set; } = Enumerable.Empty<AssetViewModel>();

    public static DownloadsPageViewModel GetViewModel(DownloadsPage? downloadsPage)
    {
        if (downloadsPage == null)
        {
            return new DownloadsPageViewModel();
        }
        return new DownloadsPageViewModel
        {
            Heading = downloadsPage.DownloadsPageContent.FirstOrDefault()?.DownloadsHeading ?? string.Empty,
            Assets = downloadsPage.DownloadsPageContent.FirstOrDefault()?.DownloadsAssets?.Select(AssetViewModel.GetViewModel)
                ?? Enumerable.Empty<AssetViewModel>()
        };
    }
}