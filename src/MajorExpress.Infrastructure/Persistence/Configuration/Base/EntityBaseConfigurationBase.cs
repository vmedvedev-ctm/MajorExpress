using MajorExpress.Domain.Entities.Base;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MajorExpress.Infrastructure.Persistence.Configuration.Base;

public class EntityBaseConfigurationBase<T> : IEntityTypeConfiguration<T>
    where T : EntityBase
{
    protected EntityBaseConfigurationBase()
    {
    }

    public virtual void Configure(EntityTypeBuilder<T> builder) => builder.HasKey(x => x.Id);
}
