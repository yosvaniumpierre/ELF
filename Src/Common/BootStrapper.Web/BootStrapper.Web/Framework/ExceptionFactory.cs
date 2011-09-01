namespace Avanade.BootStrapper.Web.Framework
{
    using System;

    using NLog;

    public static class ExceptionFactory
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        public static int RetrieveExceptionCodeId(CustomAppException customAppException)
        {
            var type = customAppException.Code.GetType();
            var memInfo = type.GetMember(customAppException.Code.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(CodeIdAttribute), false);

            return ((CodeIdAttribute)attributes[0]).CodeId;
        }

        public static CustomAppException WhenControllerInstantiationFailed(Exception innerException)
        {
            const string message = "Since the Framework will automatically instantiate Controllers, just ensure that: " +
                                       "(1) your custom controller extends the BaseController; " +
                                       "(2) more obviously, you actually implemented your controller!";
            var exception = new CustomAppException(ExceptionCode.ControllerInstantiation, message, innerException);

            return exception;
        }

        public static CustomAppException WhenControllerNonExistent(string controllerName)
        {
            var message = string.Format(
                "Controller ({0}) does not exist and as such, the URL is invalid.",
                controllerName);
            var exception = new CustomAppException(ExceptionCode.ControllerNonExistent, message, null);

            return exception;
        }

        public static CustomAppException Wrap(Exception innerException)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Wrapping exception: {0}", innerException);
            }
            var exception = new CustomAppException(ExceptionCode.Unknown, innerException)
                                {ResolutionInstruction = "None available!"};

            return exception;
        }

        #endregion Methods
    }
}