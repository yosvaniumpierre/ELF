namespace Avanade.Repository.Services.Hibernate.Security
{
    using System;
    using System.Collections.Generic;

    using Core;

    using Domain.Security;

    using NHibernate.Criterion;

    using Services.Security;

    public class UserRepository : NHibernateRepository<User>, IUserRepository
    {
        #region Properties

        public IUnitOfWork UnitOfWork
        {
            set { NHibernateUnitOfWork = value as INHibernateUnitOfWork; }
            get { return NHibernateUnitOfWork; }
        }

        #endregion Properties

        #region Methods

        public IList<User> FindUsersByEmail(string email, int pageIndex, int pageSize, string appName)
        {
            return NHibernateUnitOfWork.CurrentSession.CreateCriteria(typeof(User))
                        .Add(Restrictions.Like("Email", email, MatchMode.Anywhere))
                        .Add(Restrictions.Eq("ApplicationName", appName))
                        .SetFirstResult(pageIndex * pageSize).SetMaxResults(pageSize)
                        .List<User>();
        }

        public IList<User> FindUsersByName(string userName, int pageIndex, int pageSize, string appName)
        {
            return NHibernateUnitOfWork.CurrentSession.CreateCriteria(typeof(User))
                        .Add(Restrictions.Like("UserName", userName, MatchMode.Anywhere))
                        .Add(Restrictions.Eq("ApplicationName", appName))
                        .SetFirstResult(pageIndex * pageSize).SetMaxResults(pageSize)
                        .List<User>();
        }

        public IList<User> GetAllUsers(int pageIndex, int pageSize, string appName)
        {
            return NHibernateUnitOfWork.CurrentSession.CreateCriteria(typeof(User))
                        .Add(Restrictions.Eq("ApplicationName", appName))
                        .SetFirstResult(pageIndex * pageSize).SetMaxResults(pageSize)
                        .List<User>();
        }

        public int GetNumberOfUsersOnline(string applicationName, DateTime compareTime)
        {
            return Count(x => x.ApplicationName == applicationName && x.LastActivityDate > compareTime);
        }

        public User GetUser(string applicationName, string username)
        {
            return Single(x => x.UserName == username && x.ApplicationName == applicationName);
        }

        public User GetUserById(string applicationName, long userId)
        {
            return Single(x => x.Id == userId && x.ApplicationName == applicationName);
        }

        public string GetUserNameByEmail(string applicationName, string email)
        {
            var user = Single(x => x.Email == email && x.ApplicationName == applicationName);

            return user == null ? string.Empty : user.UserName;
        }

        #endregion Methods
    }
}