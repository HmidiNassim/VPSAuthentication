namespace VPSAuthentication.Common.Signature
{
    public class Configuration
    {
        /// <summary>
        /// VPS=Vente privé Signature
        /// Token =mail en base64 / hash ...
        /// </summary>
        public const string HeaderSignatureName = "VPS-Token";

        // X-Amz-Date pour aws et pour le moment j'ai utilisé Date du header
        //public const string HeaderDateName = "X-Vp-Date";
       
        public const string Schema = "VPS-HMAC-SHA256";

        public const int PeriodeValiditeEnMinutes = 20;

        public const string ApplicationJson = "application/json";
    }
}