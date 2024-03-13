using System.ComponentModel;

namespace TrainingGuides.Web.Features.Shared.OptionProviders.Heading;

public enum HeadingTypeOption
{
    [Description("Heading 1")]
    H1,
    [Description("Heading 2")]
    H2,
    [Description("Heading 3")]
    H3,
    [Description("Heading 4")]
    H4,
    [Description("Heading 5")]
    H5,
    [Description("Paragraph")]
    P,
}
