namespace VPSAuthentication.Common.Signature
{
    public class Configuration
    {
        /// <summary>
        /// Token = mail / hash ...
        /// </summary>
        public const string HeaderSignatureName = "VPS-Token";
       
        public const string Schema = "VPS-HMAC-SHA256";

        public const int PeriodeValiditeEnMinutes = 20;

        public const string ApplicationJson = "application/json";
    }
}