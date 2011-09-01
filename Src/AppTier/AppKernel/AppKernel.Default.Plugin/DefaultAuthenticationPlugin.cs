namespace Avanade.AppKernel.Default.Plugin
{
    using Avanade.BootStrapper.Web.AddIn;

    internal class DefaultAuthenticationPlugin : IAuthenticatePlugin
    {
        #region Methods

        public bool Verify(string authorizationHeader)
        {
            //byte[] encodedDataAsBytes = Convert.FromBase64String(authorizationHeader.Replace("Basic ", ""));
            //string val = Encoding.ASCII.GetString(encodedDataAsBytes);
            //string userpass = val;
            //string user = userpass.Substring(0, userpass.IndexOf(':'));
            //string pass = userpass.Substring(userpass.IndexOf(':') + 1);

            return true;
        }

        #endregion Methods
    }
}