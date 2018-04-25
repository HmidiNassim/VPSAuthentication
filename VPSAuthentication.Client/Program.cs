namespace VPSAuthentication.Client
{
    using Common.HttpClient;
    using Common.Models;
    using Common.Providers;
    using Common.Signature;
    using System.Net.Http;

    class Program
    {
        static void Main(string[] args)
        {

            HttpClientRequest httpClientRequest = new HttpClientRequest(new UserRepository(),
                               new CanonicalRequest(), new CalculateSignature());
            httpClientRequest.Email = "vps@gmail.com";

            var client = new HttpClient(httpClientRequest);

            var IsAuthenticated =client.GetStringAsync("http://localhost:53085/api/confidentials").GetAwaiter().GetResult();
        }
    }
}
