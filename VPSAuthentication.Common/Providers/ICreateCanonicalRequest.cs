namespace VPSAuthentication.Common.Providers
{
    using System.Net.Http;


    public interface ICreateCanonicalRequest
    {
        string CreateCanonicalRequest(HttpRequestMessage requestMessage);
    }
}