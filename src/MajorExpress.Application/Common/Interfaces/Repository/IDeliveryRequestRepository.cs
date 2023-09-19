using MajorExpress.Application.Common.Interfaces.Repository.Base;
using MajorExpress.Domain.Entities;

namespace MajorExpress.Application.Common.Interfaces.Repository;

public interface IDeliveryRequestRepository : IRepositoryBase<DeliveryRequest>
{
    Task<IReadOnlyCollection<DeliveryRequest>> SearchAsync(string tag, CancellationToken cancellationToken);
}
