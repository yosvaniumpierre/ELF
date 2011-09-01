namespace Avanade.BootStrapper.Web
{
    using System.Web;

    public sealed class BootStrapRuntime
    {
        #region Fields

        public static readonly string ExtensionMask = "*.Ext.dll";
        public static readonly string PluginMask = "*.Plugin.dll";
        public static readonly string TasksMask = "*.Tasks.dll";

        #endregion Fields

        #region Constructors

        internal BootStrapRuntime()
        {
            BinDirectory = HttpRuntime.BinDirectory;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// This is directory from which all the boot-strapping processes are conducted
        /// </summary>
        public string BinDirectory
        {
            get; private set;
        }

        #endregion Properties
    }
}