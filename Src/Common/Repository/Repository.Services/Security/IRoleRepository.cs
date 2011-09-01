namespace Avanade.Repository.Services.Security
{
    using System.Collections.Generic;

    using Domain.Security;

    public interface IRoleRepository : IRepository<Role>
    {
        #region Methods

        Role GetRole(string applicationName, string rolename);

        IList<Role> GetRoles(string applicationName);

        bool RoleExists(string applicationName, string rolename);

        #endregion Methods
    }
}