namespace Avanade.Repository.Services.Hibernate.Security
{
    using Domain.Security;

    using FluentNHibernate.Mapping;

    public sealed class ProfileMapping : ClassMap<Profile>
    {
        #region Constructors

        public ProfileMapping()
        {
            Id(x => x.Id);
            Map(x => x.ApplicationName);
            Map(x => x.BirthDate);
            Map(x => x.City);
            Map(x => x.Country);
            Map(x => x.FirstName);
            Map(x => x.Gender);
            Map(x => x.IsAnonymous);
            Map(x => x.Language);
            Map(x => x.LastActivityDate);
            Map(x => x.LastName);
            Map(x => x.LastUpdatedDate);
            Map(x => x.Occupation);
            Map(x => x.State);
            Map(x => x.Street);
            Map(x => x.Subscription);
            Map(x => x.UserId);
            Map(x => x.Website);
            Map(x => x.Zip);
        }

        #endregion Constructors
    }
}