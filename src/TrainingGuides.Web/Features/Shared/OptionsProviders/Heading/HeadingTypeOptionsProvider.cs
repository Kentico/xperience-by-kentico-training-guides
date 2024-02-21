namespace TrainingGuides.Web.Features.Shared.OptionsProviders.Heading;

public class HeadingTypeOptionsProvider : DropdownEnumOptionsProvider<HeadingTypeOptions>
{
    public HeadingTypeOptionsProvider(IEnumerable<HeadingTypeOptions>? subset = null) : base(subset)
    {
    }

    public override HeadingTypeOptions Parse(string value, HeadingTypeOptions defaultValue = HeadingTypeOptions.Auto) =>
        base.Parse(value, defaultValue);
}
