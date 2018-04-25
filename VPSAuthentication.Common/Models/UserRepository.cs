namespace VPSAuthentication.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;

    public class UserRepository : IUserRepository
    {
        ///Dictionary of user used 
        private readonly IDictionary<string, string> _passwords= new Dictionary<string, string>()
           {
                 {"vps@gmail.com","vps12345"},
                 {"nhmd@gmail.com","test123"}
           };

        private bool _disposed;

        /// <summary>
        /// check if user exit
        /// </summary>
        /// <param name="EmailAdress"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsExist(String EmailAdress,string password)
        {
            string pass = null;
            return _passwords.TryGetValue(EmailAdress,out pass) && pass.Equals(password);
        }

        /// <summary>
        /// check if user has a password then Calculate Password Hash
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public string GenerateAccessTokenUser(string Email)
        {
            if (!_passwords.ContainsKey(Email))
            {
                return null;
            }

            var userPassword = _passwords[Email];
            var hashed = CalculateHash(userPassword, new SHA1CryptoServiceProvider());
            return hashed;
        }

        /// <summary>
        /// Calculate Password Hash
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        private string CalculateHash(string inputData, HashAlgorithm algorithm)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            byte[] hashed = algorithm.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashed);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _passwords.Clear();

                }
                this._disposed = true;
            }
        }
    }
}