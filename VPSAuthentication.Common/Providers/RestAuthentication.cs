namespace VPSAuthentication.Common.Providers
{
    using Models;
    using Signature;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Tutorial utilisé
    /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/httpclient-message-handlers
    /// https://samritchie.net/2011/09/07/implementing-aws-authentication-for-your-own-rest-api/
    /// </summary>
    public class RestAuthentication
    {

        private readonly IAuthRepository _authRepository;
        private readonly ICreateCanonicalRequest _canonicalRequest;
        private readonly ICalculSignature _calculSignature;


        public RestAuthentication(IAuthRepository authRepository,
            ICreateCanonicalRequest canonicalRequest,
            ICalculSignature calculSignature)
        {
            _authRepository = authRepository;
            _canonicalRequest = canonicalRequest;
            _calculSignature = calculSignature;
        }

        public bool IsAuthenticated(HttpRequestMessage requestMessage)
        {

            if (!requestMessage.Headers.Contains(Configuration.HeaderSignatureName))
            {
                return false;
            }

            if (requestMessage.Headers.Authorization == null
                || requestMessage.Headers.Authorization.Scheme
                        != Configuration.Schema)
            {
                return false;
            }
            //AccessToken= mail crypté passé dans le header 
            string mail = requestMessage.Headers.GetValues(Configuration.HeaderSignatureName)
                                    .First();
            var accessToken = _authRepository.GenerateAccessTokenUser(mail);
            if (accessToken == null)
            {
                return false;
            }

            var canonicalRequest = _canonicalRequest.CreateCanonicalRequest(requestMessage);
            if (canonicalRequest == null)
            {
                return false;
            }

            var signature = _calculSignature.Signature(accessToken, canonicalRequest);


            var result = requestMessage.Headers.Authorization.Parameter == signature;
            return result;
        }

        private bool IsContentType(HttpRequestMessage requestMessage)
        {
            var ContentTypeHeader = requestMessage.Content.Headers.ContentType.MediaType;

            return ContentTypeHeader == Configuration.ApplicationJson;

        }

        private bool IsDateOk(HttpRequestMessage requestMessage)
        {
            var utcNow = DateTime.UtcNow;
            var date = requestMessage.Headers.Date.Value.UtcDateTime;
            if (date >= utcNow.AddMinutes(Configuration.PeriodeValiditeEnMinutes)
                || date <= utcNow.AddMinutes(-Configuration.PeriodeValiditeEnMinutes))
            {
                return false;
            }
            return true;

        }
    }
}