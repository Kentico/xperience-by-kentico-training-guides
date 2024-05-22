// using Kentico.Xperience.Admin.Base;
// using TrainingGuides.ProjectSettings;
// using TrainingGuides.Admin.ProjectSettings;
// using CMS.DataEngine;
// using CMS.ContentEngine;


// [assembly: UIPage(
//     parentType: typeof(WebChannelSettingsEditSection),
//     slug: "channel-settings",
//     uiPageType: typeof(SeoSettingsList),
//     name: "Channel settings",
//     templateName: TemplateNames.LISTING,
//     order: 0)]

// namespace TrainingGuides.Admin.ProjectSettings;
// public class SeoSettingsList : ListingPage
// {
//     private readonly IInfoProvider<ChannelInfo> channelInfoProvider;
//     private readonly IInfoProvider<WebChannelSettingsInfo> webChannelSettingsInfoProvider;

//     /// <summary>
//     /// ID of the class.
//     /// </summary>
//     [PageParameter(typeof(IntPageModelBinder))]
//     public int WebChannelSettingsId { get; set; }

//     protected override string ObjectType => WebChannelSettingsInfo.OBJECT_TYPE;

//     public SeoSettingsList(IInfoProvider<ChannelInfo> channelInfoProvider, IInfoProvider<WebChannelSettingsInfo> webChannelSettingsInfoProvider) : base()
//     {
//         this.channelInfoProvider = channelInfoProvider;
//         this.webChannelSettingsInfoProvider = webChannelSettingsInfoProvider;

//         GetOrCreateSettingsList();

//     }

//     public override async Task ConfigurePage()
//     {


//         PageConfiguration.ColumnConfigurations
//                      .AddColumn(nameof(
//                         SeoSettingsInfo.), "Channel");

//         PageConfiguration.AddEditRowAction<GlobalSettingsEditSection>();
//         // PageConfiguration.TableActions
//         //         .AddDeleteAction(nameof(Delete));

//         await base.ConfigurePage();

//     }

//     [PageCommand]
//     public override Task<ICommandResponse<RowActionResult>> Delete(int id) => base.Delete(id);


//     private void GetOrCreateSettingsList()
//     {
//         var channels = channelInfoProvider.Get();
//         var settings = webChannelSettingsInfoProvider.Get().ToList();

//         foreach (var channel in channels)
//         {
//             if (settings.Where(setting => setting.WebChannelSettingsChannel.Contains(channel.ChannelName)).Count() == 0)
//             {
//                 var newSetting = new WebChannelSettingsInfo
//                 {
//                     WebChannelSettingsChannel = [channel.ChannelName],
//                     WebChannelSettingsChannelLabel = channel.ChannelDisplayName,
//                 };
//                 webChannelSettingsInfoProvider.Set(newSetting);
//             }
//         }
//     }
// }