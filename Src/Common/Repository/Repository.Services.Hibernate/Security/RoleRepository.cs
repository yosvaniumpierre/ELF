namespace Avanade.Repository.Services.Hibernate.Security
{
    using System.Collections.Generic;

    using Core;

    using Domain.Security;

    using Services.Security;

    public class RoleRepository : NHibernateRepository<Role>, IRoleRepository
    {
        #region Methods

        public Role GetRole(string applicationName, string rolename)
        {
            return Single(x => x.RoleName == rolename && x.ApplicationName == applicationName);
        }

        public IList<Role> GetRoles(string applicationName)
        {
            return new List<Role>(Find(x => x.ApplicationName == applicationName));
        }

        public bool RoleExists(string applicationName, string rolename)
        {
            return GetRole(applicationName, rolename) != null;
        }

        #endregion Methods
    }
}