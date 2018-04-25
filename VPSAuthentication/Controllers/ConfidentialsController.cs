namespace VPSAuthentication.Controllers
{
    using Common.Models;
    using Common.Providers;
    using Common.Signature;
    using System.Web.Http;


    public class ConfidentialsController : ApiController
    {
        // GET api/Confidentials : email dans le header ou en param puis on ajoute un champs email dans RestAuthentication et  
        [HttpGet, Route("api/confidentials")]
        public bool Confidentials()
        {
            RestAuthentication restAuthentication = new RestAuthentication(new AuthRepository(),
                               new CanonicalRequest(), new CalculSignature());
            return restAuthentication.IsAuthenticated(this.Request);
        }

        /*
         * //fonctionne pas car il faut changer  RestAuthentication pour accepter en param l'email et remplacer le code qui récupere l'mail du header par le param
        // GET api/Confidentials/email : email dans le header ou en param puis on ajoute un champs email dans RestAuthentication et  
        [HttpGet, Route("api/confidentials/{email}")]
        public bool Confidentials(string email)
        {
            RestAuthentication restAuthentication = new RestAuthentication(new AuthRepository(),
                               new CanonicalRequest(), new CalculSignature(),email);
            return restAuthentication.IsAuthenticated(this.Request);
        }
        */

    }
}