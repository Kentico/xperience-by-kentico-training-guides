using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Sections;

using Microsoft.AspNetCore.Components;

[assembly: RegisterEmailSection(
    identifier: FullWidthEmailSection.IDENTIFIER,
    name: "{$FullWidthEmailSection.Name$}",
    componentType: typeof(FullWidthEmailSection),
    IconClass = "icon-l-header-text")]

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Sections;

/// <summary>
/// Basic section with one column.
/// </summary>
public partial class FullWidthEmailSection : ComponentBase
{
    /// <summary>
    /// The component identifier.
    /// </summary>
    public const string IDENTIFIER = $"Kentico.Xperience.Mjml.StarterKit.{nameof(FullWidthEmailSection)}";
}
