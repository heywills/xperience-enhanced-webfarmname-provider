using System;
using System.Collections.Generic;
using System.Text;

namespace XperienceCommunity.EnhancedWebFarmNameProvider
{
    public interface IHostSpecificNameHelper
    {
        string GetUniqueInstanceName();
    }
}
