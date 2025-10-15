using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.OneTimeCode;

public class FixArticlesController(ArticleConverter articleConverter) : Controller
{
    [HttpGet("/FixArticles")]
    public async Task<IActionResult> Test()
    {
        var attempts = await articleConverter.Convert();
        return View("~/OneTimeCode/TestView.cshtml", attempts);
    }
}