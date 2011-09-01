namespace Avanade.Config.DataSource
{
    /// <summary>
    /// Combines a boolean succes/fail flag with a error/status message.
    /// </summary>
    internal class BoolMessage
    {
        #region Fields

        /// <summary>
        /// False message.
        /// </summary>
        public static readonly BoolMessage False = new BoolMessage(false, string.Empty);

        /// <summary>
        /// True message.
        /// </summary>
        public static readonly BoolMessage True = new BoolMessage(true, string.Empty);

        /// <summary>
        /// Error message for failure, status message for success.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Success / failure ?
        /// </summary>
        public readonly bool Success;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Set the readonly fields.
        /// </summary>
        /// <param name="success">Flag for message to set.</param>
        /// <param name="message">Message to set for flag.</param>
        public BoolMessage(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        #endregion Constructors
    }
}