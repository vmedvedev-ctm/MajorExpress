using MajorExpress.Application.Common.Interfaces;
using MajorExpress.Application.Common.Interfaces.Repository;
using MajorExpress.Domain.Entities;
using MajorExpress.Domain.Enums;

namespace MajorExpress.Infrastructure.Services;

public record DeliveryRequestService(
    IDeliveryRequestRepository DeliveryRequestRepository,
    ICancelCommentRepository CancelCommentRepository) : IDeliveryRequestService
{
    public async Task<Guid> CreateAsync(
        string departureAddress,
        DateTime departureTime,
        string destinationAddress,
        DateTime destinationTime,
        Guid clientId,
        Guid cargoId,
        CancellationToken cancellationToken)
    {
        var deliveryRequest = new DeliveryRequest()
                              {
                                  DepartureAddress = departureAddress,
                                  DepartureTime = departureTime,
                                  DestinationAddress = destinationAddress,
                                  DestinationTime = destinationTime,
                                  ClientId = clientId,
                                  CargoId = cargoId
                              };

        deliveryRequest = await DeliveryRequestRepository.AddAsync(deliveryRequest, cancellationToken);

        return deliveryRequest.Id;
    }

    public async Task AssignAsync(Guid deliveryRequestId, Guid assigneeId, CancellationToken cancellationToken)
    {
        var deliveryRequest = await DeliveryRequestRepository.GetByIdAsync(deliveryRequestId, cancellationToken);

        if (deliveryRequest == null)
            throw new NullReferenceException($"Attempt to assign deliveryMan {assigneeId} to nonexistent delivery request");

        deliveryRequest.AssigneeId = assigneeId;
        deliveryRequest.Status = DeliveryRequestStatus.InProcess;
        await DeliveryRequestRepository.UpdateAsync(deliveryRequest, cancellationToken);
    }

    public async Task CancelAsync(Guid deliveryRequestId, string comment, CancellationToken cancellationToken)
    {
        var deliveryRequest = await DeliveryRequestRepository.GetByIdAsync(deliveryRequestId, cancellationToken);

        if (deliveryRequest == null) throw new NullReferenceException($"Attempt to cancel nonexistent delivery request");

        if (deliveryRequest.Status is DeliveryRequestStatus.Cancelled or DeliveryRequestStatus.Done)
            throw new Exception($"Attempt to cancel cancelled or completed delivery request {deliveryRequestId}");

        if (string.IsNullOrEmpty(comment))
            throw new ArgumentException("Attempt to cancel delivery request with null or empty comment", nameof(comment));

        var cancelComment = new CancelComment() { Comment = comment, DeliveryRequestId = deliveryRequest.Id };

        await CancelCommentRepository.AddAsync(cancelComment, cancellationToken);

        deliveryRequest.Status = DeliveryRequestStatus.Cancelled;

        await DeliveryRequestRepository.UpdateAsync(deliveryRequest, cancellationToken);
    }

    public async Task<IReadOnlyCollection<DeliveryRequest>> SearchAsync(string tag, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(tag)) throw new ArgumentException("Attempt to search delivery requests by null or empty tag", nameof(tag));

        return await DeliveryRequestRepository.SearchAsync(tag, cancellationToken);
    }

    public async Task CompleteAsync(Guid deliveryRequestId, CancellationToken cancellationToken)
    {
        var deliveryRequest = await DeliveryRequestRepository.GetByIdAsync(deliveryRequestId, cancellationToken);

        if (deliveryRequest == null)
            throw new NullReferenceException($"Attempt to complete nonexistent delivery request {deliveryRequestId}");

        if (deliveryRequest.Status is DeliveryRequestStatus.Cancelled)
            throw new Exception($"Attempt to complete cancelled request {deliveryRequestId}");

        deliveryRequest.Status = DeliveryRequestStatus.Done;

        await DeliveryRequestRepository.UpdateAsync(deliveryRequest, cancellationToken);
    }

    public async Task EditAsync(
        Guid deliveryRequestId,
        CancellationToken cancellationToken,
        string? departureAddress = null,
        DateTime? departureTime = null,
        string? destinationAddress = null,
        DateTime? destinationTime = null,
        Guid? clientId = null,
        Guid? cargoId = null)
    {
        var deliveryRequest = await DeliveryRequestRepository.GetByIdAsync(deliveryRequestId, cancellationToken);

        if (deliveryRequest == null) throw new NullReferenceException($"Attempt to edit nonexistent delivery request {deliveryRequestId}");

        if (deliveryRequest.Status is not DeliveryRequestStatus.New)
            throw new Exception($"Attempt to edit delivery request {deliveryRequestId} which status is not New");

        if (!string.IsNullOrEmpty(departureAddress)) deliveryRequest.DepartureAddress = departureAddress!;

        if (departureTime.HasValue) deliveryRequest.DepartureTime = departureTime.Value;

        if (!string.IsNullOrEmpty(destinationAddress)) deliveryRequest.DestinationAddress = destinationAddress!;

        if (destinationTime.HasValue) deliveryRequest.DestinationTime = destinationTime.Value;

        if (clientId.HasValue) deliveryRequest.ClientId = clientId.Value;

        if (cargoId.HasValue) deliveryRequest.CargoId = cargoId.Value;

        await DeliveryRequestRepository.UpdateAsync(deliveryRequest, cancellationToken);
    }

    public async Task DeleteAsync(Guid deliveryRequestId, CancellationToken cancellationToken)
    {
        var deliveryRequest = await DeliveryRequestRepository.GetByIdAsync(deliveryRequestId, cancellationToken);

        if (deliveryRequest == null)
            throw new NullReferenceException($"Attempt to delete nonexistent delivery request {deliveryRequestId}");

        if (deliveryRequest.Status is not DeliveryRequestStatus.New)
            throw new Exception($"Attempt to delete delivery request {deliveryRequestId} which status is not New");

        await DeliveryRequestRepository.RemoveAsync(deliveryRequest, cancellationToken);
    }
}
