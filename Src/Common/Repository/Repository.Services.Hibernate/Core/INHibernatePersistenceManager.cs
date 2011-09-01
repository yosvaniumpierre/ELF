namespace Avanade.Repository.Services.Hibernate.Core
{
    using NHibernate;

    public interface INHibernatePersistenceManager : IPersistenceManager
    {
        #region Properties

        ISessionFactory SessionFactory
        {
            get;
        }

        #endregion Properties
    }
}