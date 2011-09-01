namespace Avanade.Repository.Services.Hibernate.Core
{
    using NHibernate;

    public interface INHibernateUnitOfWork : IUnitOfWork
    {
        #region Properties

        ISession CurrentSession
        {
            get;
        }

        #endregion Properties
    }
}