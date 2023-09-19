using System.Globalization;

using MajorExpress.Application.Common.Interfaces.Repository;
using MajorExpress.Domain.Entities;
using MajorExpress.Infrastructure.Persistence.Repository.Base;

using Microsoft.EntityFrameworkCore;

namespace MajorExpress.Infrastructure.Persistence.Repository;

public class DeliveryRequestRepository : RepositoryBase<DeliveryRequest>, IDeliveryRequestRepository
{
    public DeliveryRequestRepository(IDbContextFactory<DatabaseContext> dbContextFactoryFactory)
        : base(dbContextFactoryFactory)
    {
    }

    public async Task<IReadOnlyCollection<DeliveryRequest>> SearchAsync(string tag, CancellationToken cancellationToken)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        var list = await dbContext.DeliveryRequests.Where(
                       x => x.DepartureAddress.Contains(tag, StringComparison.InvariantCultureIgnoreCase)
                            || x.DestinationAddress.Contains(tag, StringComparison.InvariantCultureIgnoreCase)
                            || x.Status.ToString().Contains(tag, StringComparison.CurrentCultureIgnoreCase)
                            || x.DepartureTime.ToString(CultureInfo.InvariantCulture).Contains(
                                tag,
                                StringComparison.InvariantCultureIgnoreCase)
                            || x.DestinationTime.ToString(CultureInfo.InvariantCulture).Contains(
                                tag,
                                StringComparison.InvariantCultureIgnoreCase)).ToListAsync(cancellationToken);

        return list.AsReadOnly();
    }
}
