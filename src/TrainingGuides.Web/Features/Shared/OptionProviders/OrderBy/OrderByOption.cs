using System.ComponentModel;

namespace TrainingGuides.Web.Features.Shared.OptionProviders.OrderBy;
public enum OrderByOption
{
    [Description("Newest first")]
    NewestFirst,
    [Description("Oldest first")]
    OldestFirst
}