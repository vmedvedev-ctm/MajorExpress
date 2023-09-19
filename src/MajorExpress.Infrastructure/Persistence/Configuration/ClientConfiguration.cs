using MajorExpress.Domain.Entities;
using MajorExpress.Infrastructure.Persistence.Configuration.Base;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MajorExpress.Infrastructure.Persistence.Configuration;

public class ClientConfiguration : EntityBaseConfigurationBase<Client>
{
    public override void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasMany(x => x.DeliveryRequests)
               .WithOne(x => x.Client)
               .HasForeignKey(x => x.ClientId);

        base.Configure(builder);
    }
}
