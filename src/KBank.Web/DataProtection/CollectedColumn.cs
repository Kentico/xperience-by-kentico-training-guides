namespace KBank.Web.DataProtection;

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