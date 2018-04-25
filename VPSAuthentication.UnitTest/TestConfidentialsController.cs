using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPSAuthentication.Common.HttpClient;
using VPSAuthentication.Common.Providers;
using VPSAuthentication.Common.Signature;
using VPSAuthentication.Common.Models;
using System.Net.Http;

namespace VPSAuthentication.UnitTest
{
    [TestClass]
    public class TestConfidentialsController
    {
 
        [TestMethod]
        public void TestAccessOK()
        {
            HttpClientRequest httpClientRequest = new HttpClientRequest(new AuthRepository(),
                                       new CanonicalRequest(), new CalculSignature());
            httpClientRequest.Email = "vps@gmail.com";

            var client = new HttpClient(httpClientRequest);

            bool result = client.GetStringAsync("http://localhost:53085/api/confidentials").GetAwaiter().GetResult()== "true" ? true:false;
            Assert.Equals(result, true);
        }


        [TestMethod]
        public void TestAccessNotOK()
        {
            HttpClientRequest httpClientRequest = new HttpClientRequest(new AuthRepository(),
                                       new CanonicalRequest(), new CalculSignature());
            httpClientRequest.Email = "vppps@gmail.com";

            var client = new HttpClient(httpClientRequest);

            bool result = client.GetStringAsync("http://localhost:53085/api/confidentials").GetAwaiter().GetResult() == "true" ? true : false;
            Assert.Equals(result, false);
        }

    }
}

