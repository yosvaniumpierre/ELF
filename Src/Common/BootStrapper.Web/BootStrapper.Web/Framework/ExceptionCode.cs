namespace Avanade.BootStrapper.Web.Framework
{
    using System;

    #region Enumerations

    public enum ExceptionCode
    {
        [CodeId(-1)]
        Unknown,

        [CodeId(101)]
        ControllerInstantiation,

        [CodeId(102)]
        ControllerNonExistent
    }

    #endregion Enumerations

    [AttributeUsage(AttributeTargets.Field)]
    public class CodeIdAttribute : Attribute
    {
        #region Constructors

        public CodeIdAttribute(int codeValue)
        {
            CodeId = codeValue;
        }

        #endregion Constructors

        #region Properties

        public int CodeId
        {
            get; private set;
        }

        #endregion Properties
    }
}