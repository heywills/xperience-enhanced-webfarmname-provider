// TODO: Verify WEBSITE_INSTANCE_ID is slot specific -- No. WEBSITE_INSTANCE_ID is the same across slots.
// WEBSITE_SITE_NAME is also not slot specific.
//
// WEBSITE_IIS_SITE_NAME is slot specific
// echo %WEBSITE_IIS_SITE_NAME%
// ~1starterkit-cms-dev-2022__7fa7
// echo %WEBSITE_IIS_SITE_NAME%
// ~1starterkit-cms-dev-202


using CMS.Base;
using CMS.Core;
using Microsoft.Extensions.Hosting;
using System;

namespace XperienceCommunity.EnhancedWebFarmNameProvider
{
    public class WebFarmServerNameHelper : IWebFarmServerNameHelper
    {
        private const string WEBSITE_SITE_NAME_ENV_VARIABLE = "WEBSITE_SITE_NAME";
        private const string WEBSITE_INSTANCE_ID_ENV_VARIABLE = "WEBSITE_INSTANCE_ID";
        private const string AUTO_EXTERNAL_WEBAPP_SUFFIX = "AutoExternalWeb";
        private const string INSTANCE_NAME_SUFFIX_CONFIG_KEY = "CMSInstanceNameSuffix";

        private readonly IHostEnvironment _hostEnvironment;
        private readonly IConversionService _conversionService;
        private readonly IAppSettingsService _appSettingsService;

        public WebFarmServerNameHelper(IHostEnvironment hostEnvironment,
                                       IConversionService conversionService,
                                       IAppSettingsService appSettingsService)
        {
            _hostEnvironment = hostEnvironment;
            _conversionService = conversionService;
            _appSettingsService = appSettingsService;
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
        /// <para>Additoinally, this enhancement provides web farm server names based on 
        /// Azure App Services names which are easier for admins to audit.</para>
        /// </summary>
        /// <returns><c>String Representation of the Server Name</c></returns>
        /// <remarks>Based on work provided by Brandon Henricks (https://github.com/brandonhenricks)</remarks>
        public string GetAutomaticWebFarmServerName()
        {
            var websiteName = Environment.GetEnvironmentVariable(WEBSITE_SITE_NAME_ENV_VARIABLE, EnvironmentVariableTarget.Process);
            var instanceName = Environment.GetEnvironmentVariable(WEBSITE_INSTANCE_ID_ENV_VARIABLE, EnvironmentVariableTarget.Process);

            if(!string.IsNullOrWhiteSpace(websiteName) && !string.IsNullOrWhiteSpace(instanceName))
            {
                return $"{websiteName}_{instanceName}";
            }
            return _conversionService.GetCodeName(SystemContext.MachineName + SystemContext.ApplicationPath) + GetInstanceNameSuffix();
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
            var customInstancenameSuffix = Convert.ToString(_appSettingsService[INSTANCE_NAME_SUFFIX_CONFIG_KEY]);

            if (!SystemContext.IsCMSRunningAsMainApplication && SystemContext.IsWebSite && customInstancenameSuffix == null)
            {
                customInstancenameSuffix = AUTO_EXTERNAL_WEBAPP_SUFFIX;
            }

            if (!String.IsNullOrEmpty(customInstancenameSuffix))
            {
                instanceNameSuffix = $"_{customInstancenameSuffix}";
            }
            return instanceNameSuffix;
        }

    }
}
