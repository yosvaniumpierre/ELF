namespace Avanade.Platform.Services.Web.Configuration
{
    internal interface IKeyConfigHandler
    {
        #region Methods

        IConfigSetting Get(string key);

        bool Reinitialise();

        #endregion Methods
    }
}