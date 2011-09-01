namespace Avanade.Repository.Services.Hibernate.Core
{
    using System;

    using NHibernate;

    public class NHibernateUnitOfWork : INHibernateUnitOfWork
    {
        #region Fields

        private readonly ISessionFactory sessionFactory;

        private bool isDisposed;
        private bool isInitialized;
        private ITransaction transaction;

        #endregion Fields

        #region Constructors

        public NHibernateUnitOfWork(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        #endregion Constructors

        #region Properties

        public ISession CurrentSession
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public void Commit()
        {
            ShouldNotCurrentlyBeDisposed();
            ShouldBeInitializedFirst();

            transaction.Commit();

            CurrentSession.Flush();
            transaction.Dispose();
        }

        public void Dispose()
        {
            if (isDisposed || !isInitialized)
            {
                return;
            }

            transaction.Dispose();
            CurrentSession.Dispose();

            isDisposed = true;
        }

        public void Rollback()
        {
            ShouldNotCurrentlyBeDisposed();
            ShouldBeInitializedFirst();

            transaction.Rollback();

            BeginNewTransaction();
        }

        public void Start()
        {
            ShouldNotCurrentlyBeDisposed();

            CurrentSession = sessionFactory.OpenSession();
            BeginNewTransaction();

            isInitialized = true;
        }

        private void BeginNewTransaction()
        {
            if (transaction != null)
            {
                transaction.Dispose();
            }

            transaction = CurrentSession.BeginTransaction();
        }

        private void ShouldBeInitializedFirst()
        {
            if (!isInitialized)
            {
                throw new InvalidOperationException("Must initialize (call Start()) on NHibernateUnitOfWork before commiting or rolling back");
            }
        }

        private void ShouldNotCurrentlyBeDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion Methods
    }
}