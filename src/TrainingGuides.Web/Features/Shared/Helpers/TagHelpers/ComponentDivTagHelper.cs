using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerType;
using TrainingGuides.Web.Features.Shared.OptionsProviders.ColorScheme;

namespace TrainingGuides.Web.Features.Shared.Helpers.TagHelpers;

public class ComponentDivTagHelper : TagHelper
{
    public string ColorScheme { get; set; }
    public string CornerType { get; set; }
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";

        var cssClasses = new List<string>();

        cssClasses.AddRange(GetColorSchemeClasses());
        cssClasses.AddRange(GetCornerTypeClasses());

        if (cssClasses.Count > 0)
        {
            foreach (string cssClass in cssClasses)
            {
                output.AddClass(cssClass, HtmlEncoder.Default);
            }
        }
    }

    private IEnumerable<string> GetColorSchemeClasses()
    {
        var colorScheme = GetColorScheme();
        return colorScheme switch
        {
            ColorSchemeOption.Light1 => ["tg-bg-light-1", "tg-txt-dark"],
            ColorSchemeOption.Light2 => ["tg-bg-light-2", "tg-txt-dark"],
            ColorSchemeOption.Light3 => ["tg-bg-light-3", "tg-txt-dark"],
            ColorSchemeOption.Dark1 => ["tg-bg-primary", "tg-txt-light"],
            ColorSchemeOption.Dark2 => ["tg-bg-secondary", "tg-txt-light"],
            ColorSchemeOption.TransparentLight => ["tg-bg-none", "tg-txt-light"],
            ColorSchemeOption.TransparentDark or
            ColorSchemeOption.Auto => ["tg-bg-none", "tg-txt-dark"],
            _ => [string.Empty],
        };

    }

    private IEnumerable<string> GetCornerTypeClasses()
    {
        var cornerType = GetCornerType();
        return cornerType switch
        {
            CornerTypeOption.Round => ["tg-corner-rnd"],
            CornerTypeOption.VeryRound => ["tg-corner-v-rnd"],
            CornerTypeOption.Sharp => ["tg-corner-shrp"],
            _ => [string.Empty],
        };
    }

    private CornerTypeOption GetCornerType()
    {
        if (!Enum.TryParse(CornerType, out CornerTypeOption cornerType))
        {
            cornerType = CornerTypeOption.Round;
        }

        return cornerType;
    }

    private ColorSchemeOption GetColorScheme()
    {
        if (!Enum.TryParse(ColorScheme, out ColorSchemeOption colorScheme))
        {
            colorScheme = ColorSchemeOption.TransparentDark;
        }

        return colorScheme;
    }
}
