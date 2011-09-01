namespace Avanade.Repository.Services.Security
{
    using System;
    using System.Collections.Generic;

    using Domain.Security;

    public interface IProfileRepository : IRepository<Profile>
    {
        #region Methods

        Profile GetProfile(long id, bool isAnonymous = false);

        IList<Profile> GetProfiles(string applicationName, DateTime userInactiveSinceDate, bool anaon);

        IList<Profile> GetProfiles(string applicationName, bool isAnonymous, int pageIndex, int pageSize);

        IList<Profile> GetProfiles(string applicationName, bool isAnonymous, DateTime userInactiveSinceDate, int pageIndex, int pageSize);

        #endregion Methods
    }
}