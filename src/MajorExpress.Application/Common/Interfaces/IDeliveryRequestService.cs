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

    Task AssignAsync(Guid deliveryRequestId, Guid assigneeId, CancellationToken cancellationToken);

    Task CancelAsync(Guid deliveryRequestId, string comment, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<DeliveryRequest>> SearchAsync(string tag, CancellationToken cancellationToken);

    Task CompleteAsync(Guid deliveryRequestId, CancellationToken cancellationToken);

    Task EditAsync(
        Guid deliveryRequestId,
        CancellationToken cancellationToken,
        string? departureAddress = null,
        DateTime? departureTime = null,
        string? destinationAddress = null,
        DateTime? destinationTime = null,
        Guid? clientId = null,
        Guid? cargoId = null);

    Task DeleteAsync(Guid deliveryRequestId, CancellationToken cancellationToken);
}
