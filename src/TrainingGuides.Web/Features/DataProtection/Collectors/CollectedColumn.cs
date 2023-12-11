namespace TrainingGuides.Web.Features.DataProtection.Collectors;

public class CollectedColumn
{
    public string Name { get; }

    public string DisplayName { get; }

    public CollectedColumn(string name, string displayName)
    {
        Name = name;
        DisplayName = displayName;
    }
}