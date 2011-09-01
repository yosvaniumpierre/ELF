namespace Avanade.Repository.Services.Hibernate.Security
{
    using System;
    using System.Collections.Generic;

    using Core;

    using Domain.Security;

    using NHibernate.Criterion;

    using Services.Security;

    public class ProfileRepository : NHibernateRepository<Profile>, IProfileRepository
    {
        #region Methods

        public Profile GetProfile(long id, bool isAnonymous = false)
        {
            return Single(x => x.Id == id && x.IsAnonymous == isAnonymous);
        }

        public IList<Profile> GetProfiles(string applicationName, DateTime userInactiveSinceDate, bool anaon)
        {
            return
                new List<Profile>(Find(
                    x =>
                    x.ApplicationName == applicationName && x.LastActivityDate < userInactiveSinceDate &&
                    x.IsAnonymous == anaon));
        }

        public IList<Profile> GetProfiles(string applicationName, bool isAnonymous, int pageIndex, int pageSize)
        {
            return NHibernateUnitOfWork.CurrentSession.CreateCriteria(typeof(Profile))
                        .Add(Restrictions.Like("IsAnonymous", isAnonymous))
                        .Add(Restrictions.Eq("ApplicationName", applicationName))
                        .SetFirstResult(pageIndex * pageSize).SetMaxResults(pageSize)
                        .List<Profile>();
        }

        public IList<Profile> GetProfiles(string applicationName, bool isAnonymous, DateTime userInactiveSinceDate, int pageIndex, int pageSize)
        {
            return NHibernateUnitOfWork.CurrentSession.CreateCriteria(typeof(Profile))
                        .Add(Restrictions.Like("IsAnonymous", isAnonymous))
                        .Add(Restrictions.Le("LastActivityDate", userInactiveSinceDate))
                        .Add(Restrictions.Eq("ApplicationName", applicationName))
                        .SetFirstResult(pageIndex * pageSize).SetMaxResults(pageSize)
                        .List<Profile>();
        }

        #endregion Methods
    }
}