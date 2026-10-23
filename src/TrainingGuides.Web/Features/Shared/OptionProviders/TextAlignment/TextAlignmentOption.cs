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
    Left = 0,
    Center = 1,
    Right = 2
}
