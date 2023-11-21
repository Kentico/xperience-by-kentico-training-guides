using System;
using System.Data;
using System.Runtime.Serialization;
using System.Collections.Generic;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using TrainingGuides.Admin;

[assembly: RegisterObjectType(typeof(CookieLevelConsentMappingInfo), CookieLevelConsentMappingInfo.OBJECT_TYPE)]

namespace TrainingGuides.Admin
{
    /// <summary>
    /// Data container class for <see cref="CookieLevelConsentMappingInfo"/>.
    /// </summary>
    [Serializable]
    public partial class CookieLevelConsentMappingInfo : AbstractInfo<CookieLevelConsentMappingInfo, ICookieLevelConsentMappingInfoProvider>
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "kbank.cookielevelconsentmapping";


        /// <summary>
        /// Type information.
        /// </summary>
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(CookieLevelConsentMappingInfoProvider), OBJECT_TYPE, "KBank.CookieLevelConsentMapping", "CookieLevelConsentMappingID", null, "CookieLevelConsentMappingGuid", "CookieLevelConsentMappingGuid", "CookieLevelConsentMappingID", null, null, null)
        {
            ModuleName = "KBank.Admin",
            TouchCacheDependencies = true,
            DependsOn = new List<ObjectDependency>()
            {
            },
        };


        /// <summary>
        /// Cookie level consent mapping ID.
        /// </summary>
        [DatabaseField]
        public virtual int CookieLevelConsentMappingID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(CookieLevelConsentMappingID)), 0);
            set => SetValue(nameof(CookieLevelConsentMappingID), value);
        }


        /// <summary>
        /// Preference consent code name.
        /// </summary>
        [DatabaseField]
        public virtual string mPreferenceConsentCodeName
        {
            get => ValidationHelper.GetString(GetValue(nameof(PreferenceConsentCodeName)), String.Empty);
            set => SetValue(nameof(PreferenceConsentCodeName), value, String.Empty);
        }


        /// <summary>
        /// Preference consent code name.
        /// </summary>
        public IEnumerable<string> PreferenceConsentCodeName
        {
            get => global::CMS.DataEngine.Internal.JsonDataTypeConverter.ConvertToModels<string>(mPreferenceConsentCodeName);
        }


        /// <summary>
        /// Analytical consent code name.
        /// </summary>
        [DatabaseField]
        public virtual string mAnalyticalConsentCodeName
        {
            get => ValidationHelper.GetString(GetValue(nameof(AnalyticalConsentCodeName)), String.Empty);
            set => SetValue(nameof(AnalyticalConsentCodeName), value, String.Empty);
        }


        /// <summary>
        /// Analytical consent code name.
        /// </summary>
        public IEnumerable<string> AnalyticalConsentCodeName
        {
            get => global::CMS.DataEngine.Internal.JsonDataTypeConverter.ConvertToModels<string>(mAnalyticalConsentCodeName);
        }


        /// <summary>
        /// Marketing consent code name.
        /// </summary>
        [DatabaseField]
        public virtual string mMarketingConsentCodeName
        {
            get => ValidationHelper.GetString(GetValue(nameof(MarketingConsentCodeName)), String.Empty);
            set => SetValue(nameof(MarketingConsentCodeName), value, String.Empty);
        }


        /// <summary>
        /// Marketing consent code name.
        /// </summary>
        public IEnumerable<string> MarketingConsentCodeName
        {
            get => global::CMS.DataEngine.Internal.JsonDataTypeConverter.ConvertToModels<string>(mMarketingConsentCodeName);
        }


        /// <summary>
        /// Cookie level consent mapping guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid CookieLevelConsentMappingGuid
        {
            get => ValidationHelper.GetGuid(GetValue(nameof(CookieLevelConsentMappingGuid)), Guid.Empty);
            set => SetValue(nameof(CookieLevelConsentMappingGuid), value);
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            Provider.Delete(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            Provider.Set(this);
        }


        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected CookieLevelConsentMappingInfo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="CookieLevelConsentMappingInfo"/> class.
        /// </summary>
        public CookieLevelConsentMappingInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="CookieLevelConsentMappingInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public CookieLevelConsentMappingInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}