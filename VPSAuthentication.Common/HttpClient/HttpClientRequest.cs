namespace VPSAuthentication.Common.HttpClient
{
    using Models;
    using Providers;
    using Signature;
    using System;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class HttpClientRequest : HttpClientHandler
    {
        private readonly IAuthRepository _authRepository;
        private readonly ICreateCanonicalRequest _canonicalRequest;
        private readonly ICalculSignature _calculSignature;

        public string Email { get; set; }

        public HttpClientRequest(IAuthRepository secretRepository,
                              ICreateCanonicalRequest canonicalRequest,
                              ICalculSignature signatureCalculator)
        {
            _authRepository = secretRepository;
            _canonicalRequest = canonicalRequest;
            _calculSignature = signatureCalculator;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                     System.Threading.CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(Configuration.HeaderSignatureName))
            {
                request.Headers.Add(Configuration.HeaderSignatureName, Email);
            }
            request.Headers.Date = new DateTimeOffset(DateTime.Now, DateTime.Now - DateTime.UtcNow);

            //Content toujours null dans notre cas
            //request.Headers.Add("Content-Type", Configuration.ApplicationJson);

            var canonicalRequest = _canonicalRequest.CreateCanonicalRequest(request);
            var accessToken = _authRepository.GenerateAccessTokenUser(Email);
            string signature = _calculSignature.Signature(accessToken, canonicalRequest);

            var header = new AuthenticationHeaderValue(Configuration.Schema, signature);

            request.Headers.Authorization = header;
            return base.SendAsync(request, cancellationToken);
        }
    }
}
