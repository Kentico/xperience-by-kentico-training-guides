using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using TrainingGuides.ProjectSettings;

[assembly: RegisterObjectType(typeof(WebChannelSnippetInfo), WebChannelSnippetInfo.OBJECT_TYPE)]

namespace TrainingGuides.ProjectSettings
{
    /// <summary>
    /// Data container class for <see cref="WebChannelSnippetInfo"/>.
    /// </summary>
    public partial class WebChannelSnippetInfo : AbstractInfo<WebChannelSnippetInfo, IInfoProvider<WebChannelSnippetInfo>>, IInfoWithId, IInfoWithName
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "trainingguides.webchannelsnippet";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(IInfoProvider<WebChannelSnippetInfo>), OBJECT_TYPE, "TrainingGuides.WebChannelSnippet", "WebChannelSnippetID", null, null, "WebChannelSnippetCodeName", "WebChannelSnippetDisplayName", null, null, null)
        {
            TouchCacheDependencies = true,
            DependsOn = new List<ObjectDependency>()
            {
                new ObjectDependency("WebChannelSnippetWebChannelSettingsID", "trainingguides.webchannelsettings", ObjectDependencyEnum.Required),
            },
        };


        /// <summary>
        /// Web channel snippet ID.
        /// </summary>
        [DatabaseField]
        public virtual int WebChannelSnippetID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(WebChannelSnippetID)), 0);
            set => SetValue(nameof(WebChannelSnippetID), value);
        }


        /// <summary>
        /// Web channel snippet type.
        /// </summary>
        [DatabaseField]
        public virtual string WebChannelSnippetType
        {
            get => ValidationHelper.GetString(GetValue(nameof(WebChannelSnippetType)), String.Empty);
            set => SetValue(nameof(WebChannelSnippetType), value);
        }


        /// <summary>
        /// Web channel snippet code.
        /// </summary>
        [DatabaseField]
        public virtual string WebChannelSnippetCode
        {
            get => ValidationHelper.GetString(GetValue(nameof(WebChannelSnippetCode)), String.Empty);
            set => SetValue(nameof(WebChannelSnippetCode), value);
        }


        /// <summary>
        /// Web channel snippet web channel settings ID.
        /// </summary>
        [DatabaseField]
        public virtual int WebChannelSnippetWebChannelSettingsID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(WebChannelSnippetWebChannelSettingsID)), 0);
            set => SetValue(nameof(WebChannelSnippetWebChannelSettingsID), value);
        }


        /// <summary>
        /// Web channel snippet display name.
        /// </summary>
        [DatabaseField]
        public virtual string WebChannelSnippetDisplayName
        {
            get => ValidationHelper.GetString(GetValue(nameof(WebChannelSnippetDisplayName)), String.Empty);
            set => SetValue(nameof(WebChannelSnippetDisplayName), value);
        }


        /// <summary>
        /// Web channel snippet code name.
        /// </summary>
        [DatabaseField]
        public virtual string WebChannelSnippetCodeName
        {
            get => ValidationHelper.GetString(GetValue(nameof(WebChannelSnippetCodeName)), String.Empty);
            set => SetValue(nameof(WebChannelSnippetCodeName), value);
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
        /// Creates an empty instance of the <see cref="WebChannelSnippetInfo"/> class.
        /// </summary>
        public WebChannelSnippetInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="WebChannelSnippetInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public WebChannelSnippetInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}