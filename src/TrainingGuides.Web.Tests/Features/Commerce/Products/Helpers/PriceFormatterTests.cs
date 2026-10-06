using System.Globalization;
using TrainingGuides.Web.Commerce.Products.Helpers;
using Xunit;

namespace TrainingGuides.Web.Tests.Features.Commerce.Products.Helpers;

public class PriceFormatterTests
{
    private const decimal PRICE = 1234.5m;

    // Cultures whose currency format is stable across ICU versions. Others, such as fr-FR, differ in
    // which space characters they use from one operating system to the next.
    [Theory]
    [InlineData("en-US", "$1,234.50")]
    [InlineData("es-MX", "$1,234.50")]
    public void Format_PriceInACulture_UsesThatCulturesCurrencyFormat(string cultureName, string expected) =>
        Assert.Equal(expected, PriceFormatter.Format(PRICE, CultureInfo.GetCultureInfo(cultureName)));

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
