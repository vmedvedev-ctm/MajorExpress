using MajorExpress.Domain.Entities;
using MajorExpress.Infrastructure.Persistence.Configuration.Base;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MajorExpress.Infrastructure.Persistence.Configuration;

public class DeliveryManConfiguration : EntityBaseConfigurationBase<DeliveryMan>
{
    public override void Configure(EntityTypeBuilder<DeliveryMan> builder)
    {
        builder.HasMany(x => x.DeliveryRequests)
               .WithOne(x => x.Assignee)
               .HasForeignKey(x => x.AssigneeId);

        base.Configure(builder);
    }
}
