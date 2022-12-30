namespace DotNetPonies.Test
{
    [TestClass]
    public class StatusTest
    {
        [TestMethod]
        public async Task GetStatusTest()
        {
            // https://pony.town/api2/game/status
            var client = new PonyTownClient();
            var status = await client.GetStatus();

            Assert.IsNotNull(status, "Version is null");
            Assert.AreNotEqual(status.Version, string.Empty, "Version is empty");

            Assert.IsTrue(status.Servers.Count > 0, "No server was found.");
        }
    }
}