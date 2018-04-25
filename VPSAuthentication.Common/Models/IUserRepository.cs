namespace VPSAuthentication.Common.Models
{
    using System;

    public interface IUserRepository : IDisposable
    {
        //Check if an User is exists
        bool IsExist(String EmailAdress, string Password);

        //create password hash
        string GenerateAccessTokenUser(string Email);
    }
}