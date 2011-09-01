namespace Avanade.AppKernel.Models
{
    using System;

    /// <summary>
    /// A token to be returned after authentication is successful.
    /// </summary>
    public class AuthToken
    {
        #region Properties

        public string AccessId
        {
            get;
            set;
        }

        public DateTime ExpiryTime
        {
            get;
            set;
        }

        public string Identifier
        {
            get;
            set;
        }

        public bool IsAuthenticated
        {
            get;
            set;
        }

        #endregion Properties
    }
}