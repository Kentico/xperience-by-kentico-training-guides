using System.Globalization;

namespace TrainingGuides.Web.Commerce.Products.Helpers;

/// <summary>
/// The one way a catalog price is printed, shared by the product listing and the content promotion
/// card so the same product never shows two different price strings.
/// </summary>
public static class PriceFormatter
{
    /// <summary>
    /// Formats the price as currency in the given culture, or in the current request's culture when
    /// none is given.
    /// </summary>
    public static string Format(decimal price, IFormatProvider? culture = null) =>
        price.ToString("C", culture ?? CultureInfo.CurrentCulture);
}
