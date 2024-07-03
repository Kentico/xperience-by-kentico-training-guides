using System;
using System.Data;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using TrainingGuides.ProjectSettings;

[assembly: RegisterObjectType(typeof(WebChannelSettingsInfo), WebChannelSettingsInfo.OBJECT_TYPE)]

namespace TrainingGuides.ProjectSettings
{
    /// <summary>
    /// Data container class for <see cref="WebChannelSettingsInfo"/>.
    /// </summary>
    [Serializable]
    public partial class WebChannelSettingsInfo : AbstractInfo<WebChannelSettingsInfo, IInfoProvider<WebChannelSettingsInfo>>, IInfoWithId, IInfoWithGuid
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "trainingguides.webchannelsettings";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(IInfoProvider<WebChannelSettingsInfo>), OBJECT_TYPE, "TrainingGuides.WebChannelSettings", "WebChannelSettingsID", null, "WebChannelSettingsGUID", null, "WebChannelSettingsChannelDisplayName", null, null, null)
        {
            TouchCacheDependencies = true,
            DependsOn = new List<ObjectDependency>()
            {
                new ObjectDependency("WebChannelSettingsChannelID", "cms.channel", ObjectDependencyEnum.Required),
            },
        };


        /// <summary>
        /// Web channel settings ID.
        /// </summary>
        [DatabaseField]
        public virtual int WebChannelSettingsID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(WebChannelSettingsID)), 0);
            set => SetValue(nameof(WebChannelSettingsID), value);
        }


        /// <summary>
        /// Web channel settings channel display name.
        /// </summary>
        [DatabaseField]
        public virtual string WebChannelSettingsChannelDisplayName
        {
            get => ValidationHelper.GetString(GetValue(nameof(WebChannelSettingsChannelDisplayName)), String.Empty);
            set => SetValue(nameof(WebChannelSettingsChannelDisplayName), value);
        }


        /// <summary>
        /// Web channel settings channel ID.
        /// </summary>
        [DatabaseField]
        public virtual int WebChannelSettingsChannelID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(WebChannelSettingsChannelID)), 0);
            set => SetValue(nameof(WebChannelSettingsChannelID), value);
        }


        /// <summary>
        /// Web channel settings GUID.
        /// </summary>
        [DatabaseField]
        public virtual Guid WebChannelSettingsGUID
        {
            get => ValidationHelper.GetGuid(GetValue(nameof(WebChannelSettingsGUID)), Guid.Empty);
            set => SetValue(nameof(WebChannelSettingsGUID), value);
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
        protected WebChannelSettingsInfo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="WebChannelSettingsInfo"/> class.
        /// </summary>
        public WebChannelSettingsInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="WebChannelSettingsInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public WebChannelSettingsInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}