namespace Avanade.Platform.Services.Web.Configuration
{
    using System.Collections.Generic;

    internal abstract class AbstractKeyConfigHandler : IKeyConfigHandler
    {
        #region Fields

        protected IKeyConfigHandler NextHandler;

        #endregion Fields

        #region Methods

        public IConfigSetting Get(string key)
        {
            if (HasKey(key))
            {
                return DoGet(key);
            }

            if (NextHandler != null)
            {
                return NextHandler.Get(key);
            }

            throw new KeyNotFoundException(string.Format("Unable to find matching config for key ({0})!", key));
        }

        public abstract bool Reinitialise();

        protected internal void SetNextHandler(IKeyConfigHandler nextConfigHandler)
        {
            NextHandler = nextConfigHandler;
        }

        protected abstract IConfigSetting DoGet(string key);

        protected abstract bool HasKey(string key);

        #endregion Methods
    }
}