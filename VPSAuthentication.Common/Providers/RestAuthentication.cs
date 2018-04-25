namespace VPSAuthentication.Common.Providers
{
    using Models;
    using Signature;
    using System;
    using System.Linq;
    using System.Net.Http;

    /// <summary>
    /// 
    /// </summary>
    public class RestAuthentication
    {

        private readonly IUserRepository _userRepository;
        private readonly ICreateCanonicalRequest _createCanonicalRequest;
        private readonly ICalculateSignature _calculSignature;


        public RestAuthentication(IUserRepository authRepository,
            ICreateCanonicalRequest canonicalRequest,
            ICalculateSignature calculSignature)
        {
            _userRepository = authRepository;
            _createCanonicalRequest = canonicalRequest;
            _calculSignature = calculSignature;
        }

        public bool IsAuthenticated(HttpRequestMessage requestMessage)
        {

            if (!requestMessage.Headers.Contains(Configuration.HeaderSignatureName) 
                || !requestMessage.Headers.Date.HasValue
                || requestMessage.Headers.Authorization == null
                || requestMessage.Headers.Authorization.Scheme!= Configuration.Schema
                )
            {
                return false;
            }


            //AccessToken= mail passed in the header
            string mail = requestMessage.Headers.GetValues(Configuration.HeaderSignatureName)
                                    .First();
            var accessToken = _userRepository.GenerateAccessTokenUser(mail);
            if (accessToken == null)
            {
                return false;
            }

            //test if the date is valid
            if(!IsDateOk(requestMessage))
            {
                return false;
            }

            //create CanonicalRequest 
            var canonicalRequest = _createCanonicalRequest.CreateCanonicalRequest(requestMessage);
            if (canonicalRequest == null)
            {
                return false;
            }


            var signature = _calculSignature.Signature(accessToken, canonicalRequest);

            //check if the signature is the same as signature calculated
            return requestMessage.Headers.Authorization.Parameter == signature;
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