﻿using System;
using System.Threading;
using System.Threading.Tasks;

using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Websites;
using CMS.Websites.Routing;

namespace DancingGoat
{
    /// <summary>
    ///
    /// </summary>
    public class CurrentWebsiteChannelPrimaryLanguageRetriever : ICurrentWebsiteChannelPrimaryLanguageRetriever
    {
        private readonly IWebsiteChannelContext websiteChannelContext;
        private readonly IInfoProvider<WebsiteChannelInfo> websiteChannelInfoProvider;
        private readonly IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider;


        /// <summary>
        /// Initializes an instance of the <see cref="CurrentWebsiteChannelPrimaryLanguageRetriever"/> class.
        /// </summary>
        public CurrentWebsiteChannelPrimaryLanguageRetriever(
            IWebsiteChannelContext websiteChannelContext,
            IInfoProvider<WebsiteChannelInfo> websiteChannelInfoProvider,
            IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider)
        {
            this.websiteChannelContext = websiteChannelContext;
            this.websiteChannelInfoProvider = websiteChannelInfoProvider;
            this.contentLanguageInfoProvider = contentLanguageInfoProvider;
        }

        /// <inheritdoc/>
        public async Task<string> Get(CancellationToken cancellationToken = default)
        {
            var websiteChannel = await websiteChannelInfoProvider.GetAsync(websiteChannelContext.WebsiteChannelID, cancellationToken);

            if (websiteChannel == null)
            {
                throw new InvalidOperationException($"Website channel with ID {websiteChannelContext.WebsiteChannelID} does not exist.");
            }

            var languageInfo = await contentLanguageInfoProvider.GetAsync(websiteChannel.WebsiteChannelPrimaryContentLanguageID, cancellationToken);

            if (languageInfo == null)
            {
                throw new InvalidOperationException($"Content language with ID {websiteChannel.WebsiteChannelPrimaryContentLanguageID} does not exist.");
            }

            return languageInfo.ContentLanguageName;
        }
    }
}
