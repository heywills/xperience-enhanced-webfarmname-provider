namespace EnhancedWebFarmNameProvider.Tests
{
    [TestClass]
    public class WebFarmServerNameHelperTests
    {
        [TestMethod]
        public void GetAutomaticWebFarmServerName_Returns_CMSWebFarmServerName_If_Configured()
        {
        }

        [TestMethod]
        public void GetAutomaticWebFarmServerName_Returns_Value_Created_With_Azure_Environment_Variables_If_In_Azure()
        {
        }

        [TestMethod]
        public void GetAutomaticWebFarmServerName_Returns_Value_Created_With_Machine_Environment_Variables_If_Not_In_Azure()
        {
        }

        [TestMethod]
        public void GetAutomaticWebFarmServerName_Returns_Value_With_InstanceName_If_Not_Hosted_In_Admin()
        {
        }
    }
}