using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Sections;

using Microsoft.AspNetCore.Components;

[assembly: RegisterEmailSection(
    identifier: TwoColumnEmailSection.IDENTIFIER,
    name: "{$TwoColumnsEmailSection.Name$}",
    componentType: typeof(TwoColumnEmailSection),
    IconClass = "icon-l-cols-2")]

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Sections;

/// <summary>
/// Basic section with two columns.
/// </summary>
public partial class TwoColumnEmailSection : ComponentBase
{
    /// <summary>
    /// The component identifier.
    /// </summary>
    public const string IDENTIFIER = $"Kentico.Xperience.Mjml.StarterKit.{nameof(TwoColumnEmailSection)}";
}
