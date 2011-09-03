namespace Avanade.Platform.Services.Web.Configuration
{
    using System.Collections.Generic;

    internal abstract class AbstractSectionKeyConfigHandler : ISectionKeyConfigHandler
    {
        #region Fields

        protected ISectionKeyConfigHandler NextHandler;

        #endregion Fields

        #region Methods

        public IConfigSetting Get(string section, string key)
        {
            if (HasKey(section,  key))
            {
                return DoGet(section,  key);
            }

            if (NextHandler != null)
            {
                return NextHandler.Get(section,  key);
            }

            throw new KeyNotFoundException(string.Format("Unable to find matching config for section ({0}) and key ({1})!",
                section, key));
        }

        public abstract bool Reinitialise();

        protected internal void SetNextHandler(ISectionKeyConfigHandler nextConfigHandler)
        {
            NextHandler = nextConfigHandler;
        }

        protected abstract IConfigSetting DoGet(string section, string key);

        protected abstract bool HasKey(string section, string key);

        #endregion Methods
    }
}