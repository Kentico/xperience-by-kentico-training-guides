using System.ComponentModel;

namespace TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;

public enum ColumnLayoutOption
{
    [Description("One column")]
    OneColumn = 0,
    [Description("Two columns")]
    TwoColumnEven = 1,
    [Description("Two columns - 70/30")]
    TwoColumn7030 = 2,
    [Description("Two columns - 30/70")]
    TwoColomn3070 = 3,
    [Description("Three columns")]
    ThreeColumnEven = 4,
    [Description("Three columns - 20/40/20")]
    ThreeColumn204020 = 5,
}
