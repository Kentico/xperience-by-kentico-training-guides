using CMS.Localization;
using TrainingGuides.Admin.Localization;
using TrainingGuides.Admin.Localization.Resources;

// Register resources for additional languages
[assembly: RegisterLocalizationResource(typeof(LocalizationCustom), LocalizationTarget.Server, LocalizationConstants.SpanishMXCultureCode)]
[assembly: RegisterLocalizationResource(typeof(LocalizationCustom), LocalizationTarget.Server, LocalizationConstants.FrenchCultureCode)]
[assembly: RegisterLocalizationResource(typeof(LocalizationCustom), LocalizationTarget.Server, LocalizationConstants.EnglishUSCultureCode)]
[assembly: RegisterLocalizationResource(typeof(LocalizationCustom), LocalizationTarget.Client, LocalizationConstants.SpanishMXCultureCode)]
[assembly: RegisterLocalizationResource(typeof(LocalizationCustom), LocalizationTarget.Client, LocalizationConstants.FrenchCultureCode)]
[assembly: RegisterLocalizationResource(typeof(LocalizationCustom), LocalizationTarget.Client, LocalizationConstants.EnglishUSCultureCode)]

namespace TrainingGuides.Admin.Localization.Resources;

// Class encapsulating the localization resource files
public class LocalizationCustom { }