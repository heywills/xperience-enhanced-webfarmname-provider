using CMS;
using CMS.Core;
using XperienceCommunity.EnhancedWebFarmNameProvider;

[assembly:AssemblyDiscoverable]
[assembly: RegisterImplementation(typeof(IAppSettingsService), typeof(AppSettingsServiceDecorator))]

namespace XperienceCommunity.EnhancedWebFarmNameProvider
{
    public class AppSettingsServiceDecorator : IAppSettingsService
    {
        private readonly IAppSettingsService _appSettingsService;

        public AppSettingsServiceDecorator(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }
        public string this[string key] 
        { 
            get
            {
                return _appSettingsService[key];
            } 
            set
            {
                _appSettingsService[key] = value;
            }
        }
    }
}
