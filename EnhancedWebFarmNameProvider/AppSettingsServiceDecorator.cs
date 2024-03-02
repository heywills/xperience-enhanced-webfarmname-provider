using CMS;
using CMS.Core;
using System;
using XperienceCommunity.EnhancedWebFarmNameProvider;

[assembly: RegisterImplementation(typeof(IAppSettingsService), typeof(AppSettingsServiceDecorator))]

namespace XperienceCommunity.EnhancedWebFarmNameProvider
{
    /// <summary>
    /// Decorate the IAppSettingsService to override the CMSWebFarmServerName
    /// </summary>
    public class AppSettingsServiceDecorator : IAppSettingsService
    {
        private const string KENTICO_CONFIG_CMSWEBFARMSERVERNAME = "CMSWebFarmServerName";
        private readonly IAppSettingsService _appSettingsService;
        private readonly IWebFarmServerNameHelper _webFarmServerNameHelper;

        public AppSettingsServiceDecorator(IAppSettingsService appSettingsService,
                                           IWebFarmServerNameHelper webFarmServerNameHelper)
        {
            _appSettingsService = appSettingsService;
            _webFarmServerNameHelper = webFarmServerNameHelper;
        }

        /// <summary>
        /// Get the app setting value. Override the CMSWebFarmServerName if it is empty.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key] 
        { 
            get
            {
                if (!key.Equals(KENTICO_CONFIG_CMSWEBFARMSERVERNAME, StringComparison.OrdinalIgnoreCase))
                {
                    return _appSettingsService[key];
                }
                var configuredWebFarmServerName = _appSettingsService[key];
                if (string.IsNullOrWhiteSpace(configuredWebFarmServerName))
                {
                    return _webFarmServerNameHelper.GetAutomaticWebFarmServerName();
                }
                return _appSettingsService[key];
            }
            set
            {
                _appSettingsService[key] = value;
            }
        }
    }
}
