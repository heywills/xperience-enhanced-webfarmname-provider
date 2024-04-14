using CMS;
using CMS.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var hostAddress = GetHostAddress();
            if(string.IsNullOrWhiteSpace(hostAddress))
            {
                return string.Empty;
            }
            var cleanApplicationId = hostAddress.TrimEnd('/')
                                                 .Replace("http://", string.Empty)
                                                 .Replace("https://", string.Empty)
                                                 .Replace(":", "_")
                                                 .Replace("/", "-")
                                                 .ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(cleanApplicationId))
            {
                return string.Empty;
            }
            return $"{computerName}_{cleanApplicationId}";
        }

        /// <summary>
        /// Get the server address. First, by trying to getit
        /// from server features, which will be set if hosted
        /// directly by Kestrel. If that fails, try to get it from
        /// IISServerOptions, which will be set if hosted by IIS.
        /// </summary>
        /// <returns></returns>
        private string GetHostAddress()
        {
            var address = GetKestrelHostAddress();
            if (!string.IsNullOrWhiteSpace(address))
            {
                return address;
            }
            address = GetIISHostAddress();
            return address;
        }

        /// <summary>
        /// Get the server address from the Kestrel server features.
        /// </summary>
        /// <returns></returns>
        private string GetKestrelHostAddress()
        {
            var addresses = _server.Features.Get<IServerAddressesFeature>().Addresses;
            if (addresses == null || !addresses.Any())
            {
                return string.Empty;
            }
            return addresses.First();
        }

        /// <summary>
        /// Get the server address from the IISServerOptions.
        /// </summary>
        /// <remarks>
        /// It's a pitty that the ServerAddresses property is private, but
        /// this seems like the best way to get the address when hosted by IIS.
        /// In the future, we'll be able to use a recent addition to ASP.NET Core,
        /// the IISEnvironmentFeature interface, but it requires the 8.0 ASP.NET
        /// hosting bundle to be widely distributed.
        /// See:
        /// https://github.com/dotnet/aspnetcore/issues/43632
        /// https://github.com/dotnet/aspnetcore/pull/50443
        /// </remarks>
        private string GetIISHostAddress()
        {
            var iisServerOptions = GetIISServerOptions();
            if(iisServerOptions == null)
            {
                return string.Empty;
            }
            var serverAddresses = iisServerOptions.GetType()
                                                  .GetProperty("ServerAddresses", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                                  ?.GetValue(iisServerOptions);
            if (serverAddresses is IEnumerable<string> addresses && addresses.Any())
            {
                return addresses.First();
            }
            return string.Empty;
        }

        /// <summary>
        /// Get the IISServerOptions from IServer using reflection.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Again, this is a pitty and there's a risk that the field
        /// changes. However, worst case, is that we don't create
        /// an automatic unique name and the logic fallsback to 
        /// Kentic's default implmenetation.
        /// With the .NET 8 hosting bundle, we can use the IISEnvironmentFeature
        /// instead.
        /// </remarks>
        private IISServerOptions GetIISServerOptions()
        {
            var value = _server.GetType()
                               .GetField("_options", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                               ?.GetValue(_server);
            if(value is IISServerOptions iisServerOptions)
            {
                return iisServerOptions;
            }
            return null;
        }
    }
}
