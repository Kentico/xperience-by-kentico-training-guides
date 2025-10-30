using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.OneTimeCode;

public class ConvertArticlesController(ArticleConverter articleConverter) : Controller
{
    [HttpGet("/ConvertArticles")]
    public async Task<IActionResult> Test()
    {
        var attempts = await articleConverter.Convert();
        return View("~/OneTimeCode/ConvertArticlesView.cshtml", attempts);
    }
}