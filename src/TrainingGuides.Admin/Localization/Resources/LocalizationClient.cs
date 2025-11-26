using CMS.Localization;
using TrainingGuides.Admin.Localization;
using TrainingGuides.Admin.Localization.Resources;

// Register resources for additional languages
[assembly: RegisterLocalizationResource(typeof(LocalizationClient), LocalizationTarget.Client, LocalizationConstants.SpanishMXCultureCode)]
[assembly: RegisterLocalizationResource(typeof(LocalizationClient), LocalizationTarget.Client, LocalizationConstants.FrenchCultureCode)]

namespace TrainingGuides.Admin.Localization.Resources;

// Class encapsulating the localization resource files
public class LocalizationClient { }