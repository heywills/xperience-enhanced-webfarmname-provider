using CMS;
using CMS.Base;
using CMS.Core;
using System;
using XperienceCommunity.EnhancedWebFarmNameProvider;

[assembly: RegisterImplementation(typeof(IWebFarmServerNameHelper), typeof(WebFarmServerNameHelper))]

namespace XperienceCommunity.EnhancedWebFarmNameProvider
{
    public class WebFarmServerNameHelper : IWebFarmServerNameHelper
    {
        private const string AZURE_WEBSITE_DEPLOYMENT_ID_ENV_VARIABLE = "WEBSITE_DEPLOYMENT_ID";
        private const string KENTICO_AUTO_EXTERNAL_WEBAPP_SUFFIX = "AutoExternalWeb";
        private const string KENTICO_INSTANCE_NAME_SUFFIX_CONFIG_KEY = "CMSInstanceNameSuffix";

        private readonly IConversionService _conversionService;
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IHostSpecificNameHelper _hostSpecificNameHelper;

        public WebFarmServerNameHelper(IConversionService conversionService,
                                       IConfigurationHelper configurationHelper,
                                       IHostSpecificNameHelper hostSpecificNameHelper)
        {
            _conversionService = conversionService;
            _configurationHelper = configurationHelper;
            _hostSpecificNameHelper = hostSpecificNameHelper;
        }

        /// <summary>
        /// Returns the automatic Web Farm Server Name based on environment variables. 
        /// This is an enhancement to Kentico's built-in method 
        /// <see cref="CMS.WebFarmSync.WebFarmServerInfoProvider.GetAutomaticServerName"/> to
        /// ensure the web farm server name is unique in these scenarios:
        /// <list type="bullet">
        /// <item><description> Using Azure App Services deployment slots.</description></item>
        /// <item><description> Running multiple Azure App Services in the same App Service Plan.</description></item>
        /// <item><description> Running multiple IIS websites on the same server without different 
        ///                     virtual directories (e.g., using port or host name binding).</description></item>
        /// </list>
        /// </summary>
        /// <returns><c>String Representation of the Server Name</c></returns>
        /// <remarks>Based on work provided by Brandon Henricks (https://github.com/brandonhenricks)</remarks>
        public string GetAutomaticWebFarmServerName()
        {
            var azureWebFarmServerName = GetAzureBasedWebFarmServerName();
            if(!string.IsNullOrWhiteSpace(azureWebFarmServerName))
            {
                return azureWebFarmServerName + GetInstanceNameSuffix();
            }
            var hostSpecificWebFarmServerName = _hostSpecificNameHelper.GetUniqueInstanceName(); 
            if(!string.IsNullOrWhiteSpace(hostSpecificWebFarmServerName))
            {
                return hostSpecificWebFarmServerName + GetInstanceNameSuffix();
            }
            return _conversionService.GetCodeName(SystemContext.MachineName + SystemContext.ApplicationPath) + GetInstanceNameSuffix();
        }

        /// <summary>
        /// Get a unique web farm server name based on Azure environment variable, WEBSITE_DEPLOYMENT_ID
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Test of echo %WEBSITE_IIS_SITE_NAME%_%COMPUTERNAME%
        ///    Prod Instance 9965: ~1starterkit-cms-dev-2022_DW1SDWK0000B8
        ///    Prod Instance c364: ~1starterkit-cms-dev-2022_dw1sdwk0000DO
        ///    Slot Instance 9965: ~1starterkit-cms-dev-2022__7fa7_DW1SDWK0000B8
        ///    Slot Instance c364: ~1starterkit-cms-dev-2022__7fa7_dw1sdwk0000DO
        /// WEBSITE_IIS_SITE_NAME is deprecated.
        /// Readable. Unique. Doesn't stick to instance.
        /// 
        /// Test of echo %WEBSITE_SITE_NAME%_%WEBSITE_INSTANCE_ID%
        ///     Prod Instance: starterkit-com-dev-2022_7bba8ff75d917e04579ec0c572755b5f34ebcf565d04592f367d277648232dd6
        ///     Slot Instance: starterkit-com-dev-2022_7bba8ff75d917e04579ec0c572755b5f34ebcf565d04592f367d277648232dd6
        /// Not unique. Not readable.
        /// 
        /// Test of echo %COMPUTERNAME%_%WEBSITE_DEPLOYMENT_ID%
        /// Live: DW1SDWK0000B8_bluemodus-com-prod-2023
        /// Slot: DW1SDWK0000B8_bluemodus-com-prod-2023__ba52
        /// 
        /// Readable, unique (including with mulitple scaled-up instances)
        /// but doesn't stick to slot instance. 
        /// 
        /// </remarks>
        public string GetAzureBasedWebFarmServerName()
        {
            var deploymentId = Environment.GetEnvironmentVariable(AZURE_WEBSITE_DEPLOYMENT_ID_ENV_VARIABLE, EnvironmentVariableTarget.Process);

            if (string.IsNullOrWhiteSpace(deploymentId))
            {
                return string.Empty;
            }
            var computerName = SystemContext.MachineName;
            return $"{computerName}_{deploymentId}";
        }

        /// <summary>
        /// Gets the suffix used for <see cref="GetAutomaticWebFarmServerName"/>.
        /// </summary>
        /// <remarks>
        /// This is almost a direct copy from Kentico's private property accessor for
        /// <see cref="CMS.Base.SystemContext.InstanceNameSuffix"/>.
        /// </remarks>
        public string GetInstanceNameSuffix()
        {
            var instanceNameSuffix = String.Empty;
            var customInstancenameSuffix = Convert.ToString(_configurationHelper[KENTICO_INSTANCE_NAME_SUFFIX_CONFIG_KEY]);

            if (!SystemContext.IsCMSRunningAsMainApplication && SystemContext.IsWebSite && customInstancenameSuffix == null)
            {
                customInstancenameSuffix = KENTICO_AUTO_EXTERNAL_WEBAPP_SUFFIX;
            }

            if (!String.IsNullOrEmpty(customInstancenameSuffix))
            {
                instanceNameSuffix = $"_{customInstancenameSuffix}";
            }
            return instanceNameSuffix;
        }

    }
}
