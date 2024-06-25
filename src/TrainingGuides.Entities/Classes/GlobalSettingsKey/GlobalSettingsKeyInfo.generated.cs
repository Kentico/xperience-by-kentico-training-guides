using System;
using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using TrainingGuides.ProjectSettings;

[assembly: RegisterObjectType(typeof(GlobalSettingsKeyInfo), GlobalSettingsKeyInfo.OBJECT_TYPE)]

namespace TrainingGuides.ProjectSettings
{
    /// <summary>
    /// Data container class for <see cref="GlobalSettingsKeyInfo"/>.
    /// </summary>
    [Serializable]
    public partial class GlobalSettingsKeyInfo : AbstractInfo<GlobalSettingsKeyInfo, IInfoProvider<GlobalSettingsKeyInfo>>, IInfoWithId, IInfoWithName
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "trainingguides.globalsettingskey";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(IInfoProvider<GlobalSettingsKeyInfo>), OBJECT_TYPE, "TrainingGuides.GlobalSettingsKey", "SettingsKeyID", null, null, "GlobalSettingsKeyName", "GlobalSettingsKeyDisplayName", null, null, null)
        {
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Settings key ID.
        /// </summary>
        [DatabaseField]
        public virtual int SettingsKeyID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(SettingsKeyID)), 0);
            set => SetValue(nameof(SettingsKeyID), value);
        }


        /// <summary>
        /// Global settings key value.
        /// </summary>
        [DatabaseField]
        public virtual string GlobalSettingsKeyValue
        {
            get => ValidationHelper.GetString(GetValue(nameof(GlobalSettingsKeyValue)), String.Empty);
            set => SetValue(nameof(GlobalSettingsKeyValue), value);
        }


        /// <summary>
        /// Global settings key name.
        /// </summary>
        [DatabaseField]
        public virtual string GlobalSettingsKeyName
        {
            get => ValidationHelper.GetString(GetValue(nameof(GlobalSettingsKeyName)), String.Empty);
            set => SetValue(nameof(GlobalSettingsKeyName), value);
        }


        /// <summary>
        /// Global settings key display name.
        /// </summary>
        [DatabaseField]
        public virtual string GlobalSettingsKeyDisplayName
        {
            get => ValidationHelper.GetString(GetValue(nameof(GlobalSettingsKeyDisplayName)), String.Empty);
            set => SetValue(nameof(GlobalSettingsKeyDisplayName), value);
        }


        /// <summary>
        /// Global settings key note.
        /// </summary>
        [DatabaseField]
        public virtual string GlobalSettingsKeyNote
        {
            get => ValidationHelper.GetString(GetValue(nameof(GlobalSettingsKeyNote)), String.Empty);
            set => SetValue(nameof(GlobalSettingsKeyNote), value);
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
        protected GlobalSettingsKeyInfo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="GlobalSettingsKeyInfo"/> class.
        /// </summary>
        public GlobalSettingsKeyInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="GlobalSettingsKeyInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public GlobalSettingsKeyInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}