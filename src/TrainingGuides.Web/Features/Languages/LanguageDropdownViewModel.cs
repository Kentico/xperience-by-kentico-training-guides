namespace TrainingGuides.Web.Features.Languages;

public class LanguageViewModel
{
    public string DisplayName { get; set; } = string.Empty;
    public string CurrentPageUrl { get; set; } = string.Empty;
}
public class LanguageDropdownViewModel
{
    public Dictionary<string, LanguageViewModel> AvailableLanguages { get; set; } = [];
    public string SelectedLanguage { get; set; } = string.Empty;
}