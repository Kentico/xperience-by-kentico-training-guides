using System.Globalization;
using TrainingGuides.Web.Commerce.Products.Helpers;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Commerce.Products.Helpers;

public class PriceFormatterTests
{
    private const decimal PRICE = 1234.5m;

    // Views call the formatter without a culture; it must then follow the request's culture, which
    // is what keeps the promotion card and the product listing identical.
    [Fact]
    public void Format_NoCultureGiven_UsesTheCurrentCulture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            Assert.Equal("$1,234.50", PriceFormatter.Format(PRICE));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
