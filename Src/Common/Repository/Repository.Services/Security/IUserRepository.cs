namespace Avanade.Repository.Services.Security
{
    using System;
    using System.Collections.Generic;

    using Domain.Security;

    public interface IUserRepository : IRepository<User>
    {
        #region Properties

        IUnitOfWork UnitOfWork
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        IList<User> FindUsersByEmail(string email, int pageIndex, int pageSize, string appName);

        IList<User> FindUsersByName(string userName, int pageIndex, int pageSize, string appName);

        IList<User> GetAllUsers(int pageIndex, int pageSize, string appName);

        int GetNumberOfUsersOnline(string applicationName, DateTime compareTime);

        User GetUser(string applicationName, string userName);

        User GetUserById(string applicationName, long userId);

        string GetUserNameByEmail(string applicationName, string email);

        #endregion Methods
    }
}