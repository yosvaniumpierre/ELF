namespace Avanade.BootStrapper.Web.AddIn
{
    public interface IAuthenticatePlugin
    {
        #region Methods

        bool Verify(string authorizationHeader);

        #endregion Methods
    }
}