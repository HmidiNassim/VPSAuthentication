namespace VPSAuthentication.Controllers
{
    using Common.Models;
    using Models;
    using System.Web.Http;

    public class AuthenticateController : ApiController
    {

        private IUserRepository _repo = null;

        public AuthenticateController()
        {
            _repo = new UserRepository();

        }
        // POST api/authenticate
        [HttpPost, Route("api/authenticate")]
        [AllowAnonymous]
        public bool Login(LoginModel userModel)
        {
            if (ModelState.IsValid && _repo.IsExist(userModel.Email,userModel.Password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }
      
    }
}