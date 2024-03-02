using CMS;
using CMS.Base;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;
using XperienceCommunity.EnhancedWebFarmNameProvider;
using XperienceCommunity.EnhancedWebFarmNameProvider.HostSpecificHelpers.NetCore;

[assembly: RegisterImplementation(typeof(IHostSpecificNameHelper), typeof(HostSpecificNameHelper))]

namespace XperienceCommunity.EnhancedWebFarmNameProvider.HostSpecificHelpers.NetCore
{
    /// <summary>
    /// The is the .NET 6.0+ version of the IHostSpecificNameHelper.
    /// </summary>
    /// <remarks>
    /// See the csproj file and the following page to see how this
    /// is targed.
    /// https://mcguirev10.com/2018/04/09/working-with-multitarget-solutions.html
    /// </remarks>
    public class HostSpecificNameHelper : IHostSpecificNameHelper
    {
        private readonly IServer _server;

        public HostSpecificNameHelper(IServer server)
        {
            _server = server;
        }
        /// <summary>
        /// Create a unique name for the web farm server based on the first server address
        /// and computer name.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// First Address = https://localhost:55345
        /// COMPUTERNAME = BLUEL-1459
        /// RESULT = BLUEL-1459_locahost_55345
        /// </remarks>
        public string GetUniqueInstanceName()
        {
            var computerName = SystemContext.MachineName;
            var addresses = _server.Features.Get<IServerAddressesFeature>().Addresses;
            if(addresses == null || !addresses.Any())
            {
                return string.Empty;
            }
            var firstAddress = addresses.First();
            var cleanApplicationId = firstAddress.TrimEnd('/')
                                                 .Replace("http://", string.Empty)
                                                 .Replace("https://", string.Empty)
                                                 .Replace(":", "_")
                                                 .Replace("/", "-");
            if (string.IsNullOrWhiteSpace(cleanApplicationId))
            {
                return string.Empty;
            }
            return $"{computerName}_{cleanApplicationId}";
        }
    }
}
