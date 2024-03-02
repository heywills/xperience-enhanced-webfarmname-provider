
namespace XperienceCommunity.EnhancedWebFarmNameProvider
{
    /// <summary>
    /// Wrap the framework-specific implmentation of getting and setting configuration values.
    /// In Core, it's IConfiguration. In the CMS, it's CMSAppSettings.
    /// </summary>
    public interface IConfigurationHelper
    {
        string this[string key]
        {
            get;
            set;
        }
    }
}
