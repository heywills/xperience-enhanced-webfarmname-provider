using Microsoft.Extensions.Hosting;
using System;

namespace XperienceCommunity.EnhancedWebFarmNameProvider
{
    public class WebFarmServerNameHelper : IWebFarmServerNameHelper
    {
        private const string WEBSITE_SITE_NAME_ENV_VARIABLE = "WEBSITE_SITE_NAME";
        private const string WEBSITE_INSTANCE_ID_ENV_VARIABLE = "WEBSITE_INSTANCE_ID";
        private readonly IHostEnvironment _hostEnvironment;

        public WebFarmServerNameHelper(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Returns the automatic Web Farm Server Name based on environment variables. />.
        /// </summary>
        /// <returns><c>String Representation of the Server Name</c></returns>
        /// <remarks>Provided by Brandon Henricks (https://github.com/brandonhenricks)</remarks>
        public string GetAutomaticWebFarmServerName()
        {
            var websiteName = Environment.GetEnvironmentVariable(WEBSITE_SITE_NAME_ENV_VARIABLE, EnvironmentVariableTarget.Process);
            var instanceName = Environment.GetEnvironmentVariable(WEBSITE_INSTANCE_ID_ENV_VARIABLE, EnvironmentVariableTarget.Process);

            var webFarmName = $"{websiteName}-{instanceName?.Substring(0, 6)}";

            if (!string.IsNullOrWhiteSpace(webFarmName) && webFarmName.Length > 1)
            {
                return webFarmName;
            }

            return !string.IsNullOrWhiteSpace(_hostEnvironment.ApplicationName) ? _hostEnvironment.ApplicationName : _hostEnvironment.EnvironmentName;
        }
    }
}
