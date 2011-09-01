namespace Avanade.Domain.Security
{
    using System.Collections.Generic;

    public class Role
    {
        #region Constructors

        public Role()
        {
            UsersInRole = new List<User>();
        }

        #endregion Constructors

        #region Properties

        public virtual string ApplicationName
        {
            get; set;
        }

        public virtual long Id
        {
            get; set;
        }

        public virtual string RoleName
        {
            get; set;
        }

        public virtual IList<User> UsersInRole
        {
            get; set;
        }

        #endregion Properties
    }
}