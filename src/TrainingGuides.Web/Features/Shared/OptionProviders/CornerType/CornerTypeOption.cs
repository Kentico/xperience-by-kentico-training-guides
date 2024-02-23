using System.ComponentModel;

namespace TrainingGuides.Web.Features.Shared.OptionProviders.CornerType;

public enum CornerTypeOption
{
    [Description("Sharp corners")]
    Sharp = 0,
    [Description("Round corners")]
    Round = 1,
}
