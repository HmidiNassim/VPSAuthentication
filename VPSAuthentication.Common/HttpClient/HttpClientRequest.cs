namespace VPSAuthentication.Common.HttpClient
{
    using Models;
    using Providers;
    using Signature;
    using System;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;

    public class HttpClientRequest : HttpClientHandler
    {
        //Use for Client and UnitTest projects to test ConfidentialsController  
        public string Email { get; set; }

        private IUserRepository _userRepository;

        private ICreateCanonicalRequest _createCanonicalRequest;

        private ICalculateSignature _calculSignature;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepository"> Repository user</param>
        /// <param name="canonicalRequest">allows the creation of the CanonicalRequest </param>
        /// <param name="calculateSignature"></param>
        public HttpClientRequest(IUserRepository userRepository,
                              ICreateCanonicalRequest canonicalRequest,
                              ICalculateSignature calculateSignature)
        {
            _userRepository = userRepository;
            _createCanonicalRequest = canonicalRequest;
            _calculSignature = calculateSignature;
        }


        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                     CancellationToken cancellationToken)
        {
            //add email header 
            if (!request.Headers.Contains(Configuration.HeaderSignatureName))
            {
                request.Headers.Add(Configuration.HeaderSignatureName, Email);
            }

            //set DateTime request header 
            request.Headers.Date = new DateTimeOffset(DateTime.Now, DateTime.Now - DateTime.UtcNow);

            //create CanonicalRequest
            var canonicalRequest = _createCanonicalRequest.CreateCanonicalRequest(request);

            //Generate accessToken(password hash) with mail
            var accessToken = _userRepository.GenerateAccessTokenUser(Email);

            ///Calculate Signature HMACSHA256
            string signature = _calculSignature.Signature(accessToken, canonicalRequest);

            //create  AuthenticationHeaderValue with shema and signature Authorization = VPS-HMAC-SHA256 signature
            var header = new AuthenticationHeaderValue(Configuration.Schema, signature);

            //Authorization = VPS-HMAC-SHA256 signature
            request.Headers.Authorization = header;

            //send request
            return base.SendAsync(request, cancellationToken);
        }
    }
}
