using System.ComponentModel;

namespace TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;

public enum ColumnLayoutOption
{
    [Description("One column")]
    OneColumn = 0,
    [Description("Two columns")]
    TwoColumnEven = 1,
    [Description("Two columns - Lg/Sm")]
    TwoColumnLgSm = 2,
    [Description("Two columns - Sm/Lg")]
    TwoColumnSmLg = 3,
    [Description("Three columns")]
    ThreeColumnEven = 4,
    [Description("Three columns - Sm/Lg/Sm")]
    ThreeColumnSmLgSm = 5,
}
