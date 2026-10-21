using System.ComponentModel;

namespace TrainingGuides.Web.Features.Shared.OptionProviders.TextAlignment;

/// <summary>
/// How a component aligns its text.
/// </summary>
/// <remarks>
/// <c>ServiceWidgetProperties.ContentAlignmentOption</c> is the same set of values declared
/// locally. It is deliberately left alone - see the content promotion widget spec, section 9.3.
/// </remarks>
public enum TextAlignmentOption
{
    [Description("{$TrainingGuides.Options.TextAlignment.Left$}")]
    Left = 0,

    [Description("{$TrainingGuides.Options.TextAlignment.Center$}")]
    Center = 1,

    [Description("{$TrainingGuides.Options.TextAlignment.Right$}")]
    Right = 2
}
