namespace Avanade.Repository.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPersistenceManager
    {
        #region Methods

        /// <summary>
        /// Creates an instance of IUnitOfWork for use across one or more repositories.
        /// </summary>
        /// <returns>Instance of IUnitOfWork</returns>
        IUnitOfWork Create();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="runMigrations">If true, the tables will be generated automatically</param>
        void Init(string connectionString, bool runMigrations);

        #endregion Methods
    }
}