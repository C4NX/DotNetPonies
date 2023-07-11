using System.Net.Http.Headers;

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
            await client.ResolveApiVersionAsync();
            var status = await client.GetStatusAsync();

            Assert.IsNotNull(status, "Version is null");

            Assert.IsTrue(status.Servers.Count > 0, "No server was found.");
        }
    }
}