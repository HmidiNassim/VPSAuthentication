namespace VPSAuthentication.Common.Providers
{
    using Signature;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;

    public class CanonicalRequest : ICreateCanonicalRequest
    {
        /// <summary>
        /// https://docs.aws.amazon.com/fr_fr/general/latest/gr/sigv4-create-canonical-request.html
        /// Example pseudo-code of the canonical requeste :
        /// CanonicalRequest =
        ///  HTTPRequestMethod + '\n' +
        /// CanonicalURI + '\n' +
        /// CanonicalQueryString + '\n' +
        /// CanonicalHeaders + '\n' +
        /// SignedHeaders + '\n' +
        /// HexEncode(Hash(RequestPayload))
        /// </summary>
        /// <returns></returns>
        public string CreateCanonicalRequest(HttpRequestMessage httpRequestMessage)
        {
            bool valid = IsRequestValid(httpRequestMessage);
            if (!valid)
            {
                return null;
            }

            //https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Date
            if (!httpRequestMessage.Headers.Date.HasValue)
            {
                return null;
            }
            DateTime date = httpRequestMessage.Headers.Date.Value.UtcDateTime;

            if (!httpRequestMessage.Headers.Contains(Configuration.HeaderSignatureName))
            {
                return null;
            }

            string email = httpRequestMessage.Headers
                .GetValues(Configuration.HeaderSignatureName).First();

            string httpMethod = httpRequestMessage.Method.Method + "\n";
            string CanonicalURI = httpRequestMessage.RequestUri.AbsoluteUri + "\n";
            string CanonicalHeaders = date.ToString(CultureInfo.InvariantCulture) + ";" + email.ToLower() + "\n";
            string SignedHeaders = "date;" + Configuration.HeaderSignatureName.ToLower() + "\n";

            /// CanonicalRequest =
            ///  httpMethod + '\n' +
            /// CanonicalURI + '\n' +
            /// CanonicalHeaders + '\n' +
            /// SignedHeaders + '\n' +
            /// HexEncode(Hash(RequestPayload)) no data in our case

            string canonicalRequest = String.Join("\n", httpMethod,
                CanonicalURI,
                CanonicalHeaders, SignedHeaders);

            return canonicalRequest;
        }

        private bool IsRequestValid(HttpRequestMessage requestMessage)
        {
            return true;
        }

        
    }
}