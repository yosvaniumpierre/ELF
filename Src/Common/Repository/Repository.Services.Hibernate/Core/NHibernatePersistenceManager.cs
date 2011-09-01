namespace Avanade.Repository.Services.Hibernate.Core
{
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;

    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;

    using Security;

    /// <summary>
    /// 
    /// </summary>
    public class NHibernatePersistenceManager : INHibernatePersistenceManager
    {
        #region Fields

        private bool migrations;

        #endregion Fields

        #region Properties

        public ISessionFactory SessionFactory
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public IUnitOfWork Create()
        {
            return new NHibernateUnitOfWork(SessionFactory);
        }

        public void Init(string connectionString, bool runMigrations)
        {
            migrations = runMigrations;
            SessionFactory = CreateSessionFactory(connectionString);
        }

        /// <summary>
        /// Builds the schema.
        /// </summary>
        /// <param name="config">The config.</param>
        private void BuildSchema(Configuration config)
        {
            if (migrations)
            {
                // this NHibernate tool takes a configuration (with mapping info in)
                // and exports a database schema from it
                new SchemaExport(config).Create(false, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private ISessionFactory CreateSessionFactory(string connectionString)
        {
            return Fluently.Configure()
                .Database(
                  MsSqlConfiguration.MsSql2008.ConnectionString(connectionString)
                )
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserMapping>())
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
        }

        #endregion Methods
    }
}