namespace VPSAuthentication.Common.Models
{
    using System;

    public interface IAuthRepository : IDisposable
    {
        bool IsExist(String EmailAdress, string password);
        string GenerateAccessTokenUser(string Email);
    }
}