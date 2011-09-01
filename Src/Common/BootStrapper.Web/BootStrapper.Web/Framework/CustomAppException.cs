namespace Avanade.BootStrapper.Web.Framework
{
    using System;

    public class CustomAppException : Exception
    {
        #region Constructors

        public CustomAppException(ExceptionCode exceptionCode)
        {
            Code = exceptionCode;
        }

        internal CustomAppException(ExceptionCode exceptionCode, string message, Exception exception)
            : base(message, exception)
        {
            Code = exceptionCode;
        }

        internal CustomAppException(ExceptionCode exceptionCode, Exception exception)
            : this(exceptionCode, exception.Message, exception)
        {
            //Do nothing
        }

        #endregion Constructors

        #region Properties

        public ExceptionCode Code
        {
            get; private set;
        }

        public string ResolutionInstruction
        {
            get; internal set;
        }

        #endregion Properties
    }
}