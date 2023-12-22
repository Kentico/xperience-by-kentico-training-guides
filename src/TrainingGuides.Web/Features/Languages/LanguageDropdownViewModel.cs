namespace TrainingGuides.Web.Features.Languages;

public class LanguageViewModel
{
    public string DisplayName { get; set; }
    public string CurrentPageUrl { get; set; }
}
public class LanguageDropdownViewModel
{
    public Dictionary<string, LanguageViewModel> AvailableLanguages { get; set; }
    public string SelectedLanguage { get; set; }
}