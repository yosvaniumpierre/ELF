namespace Avanade.Repository.Services.Hibernate.Security
{
    using Domain.Security;

    using FluentNHibernate.Mapping;

    public sealed class RoleMapping : ClassMap<Role>
    {
        #region Constructors

        public RoleMapping()
        {
            Id(x => x.Id);
            Map(x => x.RoleName);
            Map(x => x.ApplicationName);
            HasManyToMany(x => x.UsersInRole)
            .Cascade.All()
            .Inverse()
            .Table("UsersInRoles");
        }

        #endregion Constructors
    }
}