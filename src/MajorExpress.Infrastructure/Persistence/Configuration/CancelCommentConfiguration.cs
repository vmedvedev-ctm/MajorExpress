using MajorExpress.Domain.Entities;
using MajorExpress.Infrastructure.Persistence.Configuration.Base;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MajorExpress.Infrastructure.Persistence.Configuration;

public class CancelCommentConfiguration : EntityBaseConfigurationBase<CancelComment>
{
    public override void Configure(EntityTypeBuilder<CancelComment> builder)
    {
        builder.HasOne(x => x.DeliveryRequest)
               .WithOne();

        base.Configure(builder);
    }
}
