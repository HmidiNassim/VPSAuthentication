namespace VPSAuthentication.Controllers
{
    using Common.Models;
    using Common.Providers;
    using Common.Signature;
    using System.Web.Http;


    public class ConfidentialsController : ApiController
    {
        // GET api/Confidentials : EMail in header : VPS-Token:email
        //  
        [HttpGet, Route("api/confidentials")]
        public bool Confidentials()
        {
            RestAuthentication restAuthentication = new RestAuthentication(new UserRepository(),
                               new CanonicalRequest(), new CalculateSignature());
            return restAuthentication.IsAuthenticated(this.Request);
        }
    }
}