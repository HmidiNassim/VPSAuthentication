namespace VPSAuthentication.App_Start
{
    using Newtonsoft.Json.Serialization;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Web.Http;

    public static class FormatterConfig
    {
        /// <summary>
        /// Configures formatter to use JSON and removes XML formatter.
        /// </summary>
        /// <param name="configuration"></param>
        public static void Register(HttpConfiguration config)
        {
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}