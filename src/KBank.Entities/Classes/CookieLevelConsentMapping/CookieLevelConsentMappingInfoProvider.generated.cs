using CMS.DataEngine;

namespace KBank.Admin
{
    /// <summary>
    /// Class providing <see cref="CookieLevelConsentMappingInfo"/> management.
    /// </summary>
    [ProviderInterface(typeof(ICookieLevelConsentMappingInfoProvider))]
    public partial class CookieLevelConsentMappingInfoProvider : AbstractInfoProvider<CookieLevelConsentMappingInfo, CookieLevelConsentMappingInfoProvider>, ICookieLevelConsentMappingInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CookieLevelConsentMappingInfoProvider"/> class.
        /// </summary>
        public CookieLevelConsentMappingInfoProvider()
            : base(CookieLevelConsentMappingInfo.TYPEINFO)
        {
        }
    }
}