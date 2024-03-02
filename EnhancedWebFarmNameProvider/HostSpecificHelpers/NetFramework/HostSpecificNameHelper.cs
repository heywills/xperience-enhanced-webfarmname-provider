using CMS;
using CMS.Base;
using System.Web.Hosting;
using XperienceCommunity.EnhancedWebFarmNameProvider;
using XperienceCommunity.EnhancedWebFarmNameProvider.HostSpecificHelpers.NetFramework;

[assembly: RegisterImplementation(typeof(IHostSpecificNameHelper), typeof(HostSpecificNameHelper))]

namespace XperienceCommunity.EnhancedWebFarmNameProvider.HostSpecificHelpers.NetFramework
{
    /// <summary>
    /// The is the .NET Framework version of the IHostSpecificNameHelper.
    /// </summary>
    /// <remarks>
    /// See the csproj file and the following page to see how this
    /// is targed.
    /// https://mcguirev10.com/2018/04/09/working-with-multitarget-solutions.html
    /// </remarks>
    public class HostSpecificNameHelper : IHostSpecificNameHelper
    {
        /// <summary>
        /// Create a unique name for the web farm server based on the IIS website ApplicationID
        /// and computer name.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// .NET Framwork, HostingEnvironment
        /// ApplicationID = /LM/W3SVC/1/ROOT/kx13-dancing-goat-sample_Admin
        /// COMPUTERNAME = BLUEL-1459
        /// RESULT = BLUEL-1459_1_ROOT_kx13-dancing-goat-sample_Admin
        /// </remarks>
        public string GetUniqueInstanceName()
        {
            var computerName = SystemContext.MachineName;
            var applicationId = HostingEnvironment.ApplicationID;
            if (string.IsNullOrWhiteSpace(applicationId))
            {
                return string.Empty;
            }
            var cleanApplicationId = applicationId.Replace("/LM/W3SVC/", string.Empty)
                                                  .Replace("/", "-");
            if (string.IsNullOrWhiteSpace(cleanApplicationId))
            {
                return string.Empty;
            }
            return $"{computerName}_{cleanApplicationId}";
        }
    }
}
