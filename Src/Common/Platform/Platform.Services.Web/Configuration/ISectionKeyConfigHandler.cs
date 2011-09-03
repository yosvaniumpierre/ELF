namespace Avanade.Platform.Services.Web.Configuration
{
    internal interface ISectionKeyConfigHandler
    {
        #region Methods

        IConfigSetting Get(string section, string key);

        bool Reinitialise();

        #endregion Methods
    }
}