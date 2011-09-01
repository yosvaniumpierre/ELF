using Avanade.Domain.Util;
using FluentNHibernate.Mapping;

namespace Avanade.Repository.Services.Hibernate.Common
{
    public class ConfigItemMapping : ClassMap<ConfigItem>
    {
        public ConfigItemMapping()
        {
            Id(x => x.Id);
            Map(x => x.Key);
            Map(x => x.Value);
            Map(x => x.Category);
            Map(x => x.Comments);
        }
    }
}
