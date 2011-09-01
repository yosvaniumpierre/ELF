namespace Avanade.Repository.Services
{
    using System;

    /// <summary>
    /// A pattern that maintains a list of objects affected by a business transaction and coordinates 
    /// the writing out of changes and the resolution of concurrency problems.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        #region Methods

        void Commit();

        void Rollback();

        void Start();

        #endregion Methods
    }
}