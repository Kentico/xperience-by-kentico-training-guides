using CMS.Localization;
using TrainingGuides.Admin.Localization;
using TrainingGuides.Admin.Localization.Resources;

// Register resources for additional languages
[assembly: RegisterLocalizationResource(typeof(LocalizationBuilder), LocalizationTarget.Builder, LocalizationConstants.SpanishMXCultureCode)]
[assembly: RegisterLocalizationResource(typeof(LocalizationBuilder), LocalizationTarget.Builder, LocalizationConstants.FrenchCultureCode)]

namespace TrainingGuides.Admin.Localization.Resources;

// Class encapsulating the localization resource files
public class LocalizationBuilder { }