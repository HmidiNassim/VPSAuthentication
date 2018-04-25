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
        /// Exemple pseudo-code de la demande canonique :
        /// CanonicalRequest =
        ///  HTTPRequestMethod + '\n' +
        /// CanonicalURI + '\n' +
        /// CanonicalQueryString + '\n' +
        /// CanonicalHeaders + '\n' +
        /// SignedHeaders + '\n' +
        /// HexEncode(Hash(RequestPayload))
        /// </summary>
        /// <returns></returns>
        public string CreateCanonicalRequest(HttpRequestMessage requestMessage)
        {
            bool valid = IsRequestValid(requestMessage);
            if (!valid)
            {
                return null;
            }

            //https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Date
            if (!requestMessage.Headers.Date.HasValue)
            {
                return null;
            }
            DateTime date = requestMessage.Headers.Date.Value.UtcDateTime;

            /* le body c'est vide dans conten toujours  null
            string ContentType = requestMessage.Content==null ||requestMessage.Content.Headers.ContentType==null ? ""
                : requestMessage.Content.Headers.ContentType.MediaType;
                */
            if (!requestMessage.Headers.Contains(Configuration.HeaderSignatureName))
            {
                return null;
            }

            //email il est en base64 dans le header
            string email = requestMessage.Headers
                .GetValues(Configuration.HeaderSignatureName).First();

            string httpMethod = requestMessage.Method.Method + "\n";
            string CanonicalURI = requestMessage.RequestUri.AbsoluteUri + "\n";
            //string CanonicalQueryString = "";
            string CanonicalHeaders = date.ToString(CultureInfo.InvariantCulture) + ";" + email + "\n";//ContentType+";"+ date.ToString(CultureInfo.InvariantCulture)+ ";"+email + "\n";
            string SignedHeaders = "date;" + Configuration.HeaderSignatureName.ToLower() + "\n";// "content-type;date;" + Configuration.HeaderSignatureName.ToLower()+ "\n";
            //string RequestPayload = ""; 

            // vous devrez peut-être ajouter plus d'en-têtes si c'est nécessaire pour des raisons de sécurité
            /// CanonicalRequest =
            ///  httpMethod + '\n' +
            /// CanonicalURI + '\n' +
            /// CanonicalQueryString + '\n' +
            /// CanonicalHeaders + '\n' +
            /// SignedHeaders + '\n' +
            /// HexEncode(Hash(RequestPayload))
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