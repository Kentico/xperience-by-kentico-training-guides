using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Contracts;

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;

/// <summary>
/// Defines a mapper for converting strongly-typed email content into the email data contract.
/// Implementations should use <see cref="EmailContext.GetEmail{T}(CancellationToken)"/> to retrieve strongly-typed
/// email data and map it to the <see cref="IEmailData"/> contract.
/// </summary>
public interface IEmailDataMapper
{
    /// <summary>
    /// Maps strongly-typed email content to the email data contract.
    /// </summary>
    /// <returns>An instance of <see cref="IEmailData"/> containing the mapped email data.</returns>
    Task<IEmailData> Map();
}
