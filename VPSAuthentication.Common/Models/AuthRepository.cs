namespace VPSAuthentication.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;

    public class AuthRepository : IAuthRepository
    {
        private readonly IDictionary<string, string> _userPasswords
     = new Dictionary<string, string>()
           {
                  {"vps@gmail.com","vps12345"},
                 {"nhmd@gmail.com","test123"}
           };

        private bool _disposed;


        public bool IsExist(String EmailAdress,string password)
        {
            string pass = null;
            return _userPasswords.TryGetValue(EmailAdress,out pass) && pass.Equals(password);
        }
        
        public string GenerateAccessTokenUser(string Email)
        {
            if (!_userPasswords.ContainsKey(Email))
            {
                return null;
            }

            var userPassword = _userPasswords[Email];
            var hashed = CalculerHash(userPassword, new SHA1CryptoServiceProvider());
            return hashed;
        }

        private string CalculerHash(string inputData, HashAlgorithm algorithm)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            byte[] hashed = algorithm.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashed);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _userPasswords.Clear();

                }
                this._disposed = true;
            }
        }
    }
}