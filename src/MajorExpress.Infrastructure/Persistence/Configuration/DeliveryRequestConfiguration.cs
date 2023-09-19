using MajorExpress.Domain.Entities;
using MajorExpress.Infrastructure.Persistence.Configuration.Base;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MajorExpress.Infrastructure.Persistence.Configuration;

public class DeliveryRequestConfiguration : EntityBaseConfigurationBase<DeliveryRequest>
{
    public override void Configure(EntityTypeBuilder<DeliveryRequest> builder)
    {
        builder.HasOne(x => x.Cargo)
               .WithOne();

        base.Configure(builder);
    }
}
