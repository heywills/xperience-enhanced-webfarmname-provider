using CMS;
using Microsoft.Extensions.Configuration;
using XperienceCommunity.EnhancedWebFarmNameProvider;
using XperienceCommunity.EnhancedWebFarmNameProvider.HostSpecificHelpers.NetCore;

[assembly: RegisterImplementation(typeof(IConfigurationHelper), typeof(ConfigurationHelper))]

namespace XperienceCommunity.EnhancedWebFarmNameProvider.HostSpecificHelpers.NetCore
{
    /// <summary>
    /// Wrap the framework-specific implmentation of getting and setting configuration values,
    /// for the .NET Core-based Xperience Web applications.
    /// </summary>
    public class ConfigurationHelper : IConfigurationHelper
    {
        private readonly IConfiguration _configuration;

        public ConfigurationHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string this[string key]
        {
            get
            {
                return _configuration[key];
            }

            set
            {
                _configuration[key] = value;
            }
        }
    }
}
