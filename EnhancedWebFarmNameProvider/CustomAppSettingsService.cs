using CMS;
using CMS.Core;
using System;
using XperienceCommunity.EnhancedWebFarmNameProvider;

[assembly: RegisterImplementation(typeof(IAppSettingsService), typeof(CustomAppSettingsService))]

namespace XperienceCommunity.EnhancedWebFarmNameProvider
{
    /// <summary>
    /// Custom version of the IAppSettingsService to override the CMSWebFarmServerName
    /// </summary>
    /// <remarks>
    /// It's a pitty. Xperience 13 does not allow decorating its core services, so
    /// only one customized version of IAppSettingsService can be used.
    /// Unfortuantely, a static method uses the default implementation of IAppSettingsService
    /// to get the CMSWebFarmServerName, so we need to override the default implementation.
    /// This prevents any other component from overriding IAppSettingsService.
    /// </remarks>
    public class CustomAppSettingsService : IAppSettingsService
    {
        private const string KENTICO_CONFIG_CMSWEBFARMSERVERNAME = "CMSWebFarmServerName";
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IWebFarmServerNameHelper _webFarmServerNameHelper;

        public CustomAppSettingsService(IConfigurationHelper configurationHelper,
                                        IWebFarmServerNameHelper webFarmServerNameHelper)
        {
            _configurationHelper = configurationHelper;
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
                    return _configurationHelper[key];
                }
                var configuredWebFarmServerName = _configurationHelper[key];
                if (string.IsNullOrWhiteSpace(configuredWebFarmServerName))
                {
                    return _webFarmServerNameHelper.GetAutomaticWebFarmServerName();
                }
                return _configurationHelper[key];
            }
            set
            {
                _configurationHelper[key] = value;
            }
        }
    }
}
