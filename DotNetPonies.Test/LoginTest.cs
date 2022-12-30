using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPonies.Test
{
    [TestClass]
    public class LoginTest
    {
        private SecretData _secretData;

        public LoginTest() {
            _secretData = new SecretData(".secret.json");
        }

        [TestMethod]
        public async Task LoginWithCookieTest()
        {
            var client = new PonyTownClient()
                .LoginWithCookie(_secretData.GetSecret("PONYTOWN_COOKIE"));

            var pony = await client.GetCharactersAsync("60399261f1565c388261c07c", "NX");
            Assert.IsNotNull(pony);
        }
    }
}
