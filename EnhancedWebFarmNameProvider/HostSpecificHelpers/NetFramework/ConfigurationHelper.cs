using CMS;
using CMS.Base;
using XperienceCommunity.EnhancedWebFarmNameProvider.HostSpecificHelpers.NetFramework;
using XperienceCommunity.EnhancedWebFarmNameProvider;

[assembly: RegisterImplementation(typeof(IConfigurationHelper), typeof(ConfigurationHelper))]

namespace XperienceCommunity.EnhancedWebFarmNameProvider.HostSpecificHelpers.NetFramework
{
    /// <summary>
    /// Wrap the framework-specific implmentation of getting and setting configuration values
    /// for the .NET Framework-based CMS application.
    /// </summary>
    public class ConfigurationHelper : IConfigurationHelper
    {
        public string this[string key]
        {
            get
            {
                return SettingsHelper.AppSettings[key];
            }
            set
            {
                SettingsHelper.AppSettings[key] = value;
            }
        }
    }
}
