using MajorExpress.Domain.Entities;

namespace MajorExpress.Application.Common.Interfaces;

public interface IDeliveryRequestService
{
    Task<Guid> CreateAsync(
        string departureAddress,
        DateTime departureTime,
        string destinationAddress,
        DateTime destinationTime,
        Guid clientId,
        Guid cargoId,
        CancellationToken cancellationToken);

    Task<Guid> AssignAsync(Guid deliveryRequestId, Guid assigneeId, CancellationToken cancellationToken);

    Task<Guid> CancelAsync(Guid deliveryRequestId, string comment, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<DeliveryRequest>> SearchAsync(string tag, CancellationToken cancellationToken);
}
