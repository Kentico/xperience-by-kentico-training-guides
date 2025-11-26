using CMS.Localization;
using TrainingGuides.Admin.Localization;
using TrainingGuides.Admin.Localization.Resources;

// Register resources for additional languages
[assembly: RegisterLocalizationResource(typeof(LocalizationServer), LocalizationTarget.Server, LocalizationConstants.SpanishMXCultureCode)]
[assembly: RegisterLocalizationResource(typeof(LocalizationServer), LocalizationTarget.Server, LocalizationConstants.FrenchCultureCode)]

namespace TrainingGuides.Admin.Localization.Resources;

// Class encapsulating the localization resource files
public class LocalizationServer { }