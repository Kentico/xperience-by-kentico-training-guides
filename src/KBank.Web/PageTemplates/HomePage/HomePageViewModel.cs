using CMS.DocumentEngine.Types.KBank;
using KBank.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace KBank.Web.PageTemplates;

public class HomePageViewModel
{
    public string Message { get; set; }

    public static HomePageViewModel GetViewModel(HomePage homePage)
    {
        return new HomePageViewModel
        {
            Message = homePage.HomePageMessage
        };
    }
}

