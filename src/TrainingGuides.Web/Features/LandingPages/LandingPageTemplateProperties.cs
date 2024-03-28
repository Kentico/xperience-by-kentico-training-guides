using System.ComponentModel;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.Heading;

namespace TrainingGuides.Web.Features.LandingPages;

public class LandingPageTemplateProperties : IPageTemplateProperties
{
    private const string DESCRIPTION = "Select the type of Html tag that wraps the home page message";

    [DropDownComponent(
        Label = "Message tag type",
        ExplanationText = DESCRIPTION,
        DataProviderType = typeof(DropdownEnumOptionProvider<LandingPageHeadingTypeOption>)
    )]
    public string MessageType { get; set; } = nameof(LandingPageHeadingTypeOption.H2);
}
public enum LandingPageHeadingTypeOption
{
    [Description("Heading 1")]
    H1 = HeadingTypeOption.H1,
    [Description("Heading 2")]
    H2 = HeadingTypeOption.H2,
    [Description("Heading 3")]
    H3 = HeadingTypeOption.H3,
    [Description("Heading 4")]
    H4 = HeadingTypeOption.H4,
    [Description("Paragraph")]
    P = HeadingTypeOption.P
}

