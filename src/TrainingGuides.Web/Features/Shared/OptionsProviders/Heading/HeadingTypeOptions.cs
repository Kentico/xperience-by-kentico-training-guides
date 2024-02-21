using System.ComponentModel;

namespace TrainingGuides.Web.Features.Shared.OptionsProviders.Heading;

//TODO: options vs enum suffix vs no suffix??
//description?
public enum HeadingTypeOptions
{
    Auto,
    [Description("H1")]
    h1,
    [Description("H2")]
    h2,
    h3,
    h4,
    h5,
}
