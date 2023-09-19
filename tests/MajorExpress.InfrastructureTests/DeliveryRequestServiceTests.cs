using FluentAssertions;

using MajorExpress.Application.Common.Interfaces;
using MajorExpress.Application.Common.Interfaces.Repository;
using MajorExpress.Domain.Entities;
using MajorExpress.Domain.Enums;
using MajorExpress.Infrastructure.Services;

using NSubstitute;

namespace MajorExpress.InfrastructureTests;

public class DeliveryRequestServiceTests
{
    private readonly IDeliveryRequestRepository _deliveryRequestRepository = Substitute.For<IDeliveryRequestRepository>();

    private readonly ICancelCommentRepository _cancelCommentRepository = Substitute.For<ICancelCommentRepository>();

    private readonly IDeliveryRequestService _deliveryRequestService;

    public DeliveryRequestServiceTests() =>
        _deliveryRequestService = new DeliveryRequestService(_deliveryRequestRepository, _cancelCommentRepository);

    [Fact]
    public async Task CreateAsync_ShouldCreateNewRequest_ValidParameters()
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();

        _deliveryRequestRepository.AddAsync(
            Arg.Is<DeliveryRequest>(
                d => d.DepartureAddress == deliveryRequest.DepartureAddress
                     && d.DepartureTime == deliveryRequest.DepartureTime
                     && d.DestinationAddress == deliveryRequest.DestinationAddress
                     && d.DestinationTime == deliveryRequest.DestinationTime
                     && d.ClientId == deliveryRequest.ClientId
                     && d.CargoId == deliveryRequest.CargoId),
            default).Returns(Task.FromResult(deliveryRequest));

        // Act
        var result = await _deliveryRequestService.CreateAsync(
                         deliveryRequest.DepartureAddress,
                         deliveryRequest.DepartureTime,
                         deliveryRequest.DestinationAddress,
                         deliveryRequest.DestinationTime,
                         deliveryRequest.ClientId,
                         deliveryRequest.CargoId,
                         default);

        // Assert
        result.Should().Be(deliveryRequest.Id);
    }

    [Fact]
    public async Task AssignAsync_ShouldChangeStatus_ValidArguments()
    {
        // Arrange
        var assigneeId = Guid.NewGuid();
        var deliveryRequest = GetTestDeliveryRequest();

        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        await _deliveryRequestService.AssignAsync(deliveryRequest.Id, assigneeId, default);

        // Assert
        deliveryRequest.Status.Should().Be(DeliveryRequestStatus.InProcess);
    }

    [Fact]
    public async Task AssignAsync_ShouldAddAssignee_ValidArguments()
    {
        // Arrange
        var assigneeId = Guid.NewGuid();
        var deliveryRequest = GetTestDeliveryRequest();

        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        await _deliveryRequestService.AssignAsync(deliveryRequest.Id, assigneeId, default);

        // Assert
        deliveryRequest.AssigneeId.Should().Be(assigneeId);
    }

    [Fact]
    public async Task AssignAsync_ShouldThrowsNullRefException_WhenInvalidDeliveryRequestId()
    {
        // Arrange
        var assigneeId = Guid.NewGuid();
        var deliveryRequestId = Guid.NewGuid();
        _deliveryRequestRepository.GetByIdAsync(deliveryRequestId, default)!.Returns(ValueTask.FromResult<DeliveryRequest>(null!));

        // Act
        var action = () => _deliveryRequestService.AssignAsync(deliveryRequestId, assigneeId, default);

        // Assert
        await action.Should().ThrowExactlyAsync<NullReferenceException>()
                    .WithMessage($"Attempt to assign deliveryMan {assigneeId} to nonexistent delivery request");
    }

    [Fact]
    public async Task CancelAsync_ShouldChangeRequestStatus_ValidArguments()
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();
        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        await _deliveryRequestService.CancelAsync(deliveryRequest.Id, "test", default);

        // Assert
        deliveryRequest.Status.Should().Be(DeliveryRequestStatus.Cancelled);
    }

    [Fact]
    public async Task CancelAsync_ShouldAddCancelCommitToDb_ValidArguments()
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();
        var cancelCommentText = "test";
        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        await _deliveryRequestService.CancelAsync(deliveryRequest.Id, cancelCommentText, default);

        // Assert
        await _cancelCommentRepository.Received(1).AddAsync(
            Arg.Is<CancelComment>(x => x.Comment == cancelCommentText && x.DeliveryRequestId == deliveryRequest.Id),
            default);
    }

    [Fact]
    public async Task CancelAsync_ShouldThrowsNullRefException_WhenDeliveryRequestIdInvalid()
    {
        // Arrange
        var deliveryRequestId = Guid.NewGuid();
        _deliveryRequestRepository.GetByIdAsync(deliveryRequestId, default)!.Returns(ValueTask.FromResult<DeliveryRequest>(null!));

        // Act
        var action = () => _deliveryRequestService.CancelAsync(deliveryRequestId, "test", default);

        // Assert
        await action.Should().ThrowExactlyAsync<NullReferenceException>()
                    .WithMessage($"Attempt to cancel nonexistent delivery request");
    }

    [Fact]
    public async Task CancelAsync_ShouldThrowsException_WhenStatusIsCancelled()
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();
        deliveryRequest.Status = DeliveryRequestStatus.Cancelled;
        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        var action = () => _deliveryRequestService.CancelAsync(deliveryRequest.Id, "test", default);

        // Assert
        await action.Should().ThrowExactlyAsync<Exception>()
                    .WithMessage($"Attempt to cancel cancelled or completed delivery request {deliveryRequest.Id}");
    }

    [Fact]
    public async Task CancelAsync_ShouldThrowsException_WhenStatusIsDone()
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();
        deliveryRequest.Status = DeliveryRequestStatus.Done;
        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        var action = () => _deliveryRequestService.CancelAsync(deliveryRequest.Id, "test", default);

        // Assert
        await action.Should().ThrowExactlyAsync<Exception>()
                    .WithMessage($"Attempt to cancel cancelled or completed delivery request {deliveryRequest.Id}");
    }

    [Fact]
    public async Task CancelAsync_ShouldThrowsArgumentException_WhenCommentIsNull()
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();
        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        var action = () => _deliveryRequestService.CancelAsync(deliveryRequest.Id, null!, default);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentException>()
                    .WithMessage("Attempt to cancel delivery request with null or empty comment (Parameter 'comment')");
    }

    [Fact]
    public async Task CancelAsync_ShouldThrowsArgumentException_WhenCommentIsEmpty()
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();
        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        var action = () => _deliveryRequestService.CancelAsync(deliveryRequest.Id, string.Empty, default);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentException>()
                    .WithMessage("Attempt to cancel delivery request with null or empty comment (Parameter 'comment')");
    }

    [Fact]
    public async Task SearchAsync_ShouldThrowsArgumentException_WhenTagIsNull()
    {
        // Arrange
        string tag = null;

        // Act
        var action = () => _deliveryRequestService.SearchAsync(tag, default);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentException>()
                    .WithMessage("Attempt to search delivery requests by null or empty tag (Parameter 'tag')");
    }

    [Fact]
    public async Task SearchAsync_ShouldThrowsArgumentException_WhenTagIsEmpty()
    {
        // Arrange
        var tag = string.Empty;

        // Act
        var action = () => _deliveryRequestService.SearchAsync(tag, default);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentException>()
                    .WithMessage("Attempt to search delivery requests by null or empty tag (Parameter 'tag')");
    }

    [Fact]
    public async Task CompleteAsync_ShouldChangeRequestStatus_ValidArguments()
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();
        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        await _deliveryRequestService.CompleteAsync(deliveryRequest.Id, default);

        // Assert
        deliveryRequest.Status.Should().Be(DeliveryRequestStatus.Done);
    }

    [Fact]
    public async Task CompleteAsync_ShouldThrowsNullRefException_WhenDeliveryRequestIdIsInvalid()
    {
        // Arrange
        var deliveryRequestId = Guid.NewGuid();
        _deliveryRequestRepository.GetByIdAsync(deliveryRequestId, default)!.Returns(ValueTask.FromResult<DeliveryRequest>(null!));

        // Act
        var action = () => _deliveryRequestService.CompleteAsync(deliveryRequestId, default);

        // Assert
        await action.Should().ThrowExactlyAsync<NullReferenceException>()
                    .WithMessage($"Attempt to complete cancelled request {deliveryRequestId}");
    }

    [Fact]
    public async Task CompleteAsync_ShouldThrowsException_WhenRequestStatusIsCancelled()
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();
        deliveryRequest.Status = DeliveryRequestStatus.Cancelled;
        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        var action = () => _deliveryRequestService.CompleteAsync(deliveryRequest.Id, default);

        // Assert
        await action.Should().ThrowExactlyAsync<Exception>().WithMessage($"Attempt to complete cancelled request {deliveryRequest.Id}");
    }

    [Fact]
    public async Task EditAsync_ShouldEditRequestWithNotNullArgs_ValidArguments()
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();
        var newDestinationTime = DateTime.Now;
        var newDepartureAddress = "Changed";
        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        await _deliveryRequestService.EditAsync(
            deliveryRequest.Id,
            default,
            newDepartureAddress,
            destinationTime: newDestinationTime);

        // Assert
        await _deliveryRequestRepository.Received(1).UpdateAsync(
            Arg.Is<DeliveryRequest>(x => x.DepartureAddress == newDepartureAddress && x.DestinationTime == newDestinationTime),
            default);
    }

    [Fact]
    public async Task EditAsync_ShouldTrowsNullRefException_WhenDeliveryRequestIdInvalid()
    {
        // Arrange
        var deliveryRequestId = Guid.NewGuid();
        _deliveryRequestRepository.GetByIdAsync(deliveryRequestId, default)!.Returns(ValueTask.FromResult<DeliveryRequest>(null!));

        // Act
        var action = () => _deliveryRequestService.EditAsync(deliveryRequestId, default);

        // Assert
        await action.Should().ThrowExactlyAsync<NullReferenceException>()
                    .WithMessage($"Attempt to edit nonexistent delivery request {deliveryRequestId}");
    }

    [Theory]
    [InlineData(DeliveryRequestStatus.Cancelled)]
    [InlineData(DeliveryRequestStatus.InProcess)]
    [InlineData(DeliveryRequestStatus.Done)]
    public async Task EditAsync_ShouldThrowsException_WhenRequestStatusNotNew(DeliveryRequestStatus status)
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();
        deliveryRequest.Status = status;
        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        var action = () => _deliveryRequestService.EditAsync(deliveryRequest.Id, default);

        // Assert
        await action.Should().ThrowExactlyAsync<Exception>()
                    .WithMessage($"Attempt to edit delivery request {deliveryRequest.Id} which status is not New");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteRequest_ValidArguments()
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();
        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        await _deliveryRequestService.DeleteAsync(deliveryRequest.Id, default);

        // Assert
        await _deliveryRequestRepository.Received(1).RemoveAsync(deliveryRequest, default);
    }

    [Fact]
    public async Task DeleteAsync_ShouldTrowsNullRefException_WhenDeliveryRequestIdInvalid()
    {
        // Arrange
        var deliveryRequestId = Guid.NewGuid();
        _deliveryRequestRepository.GetByIdAsync(deliveryRequestId, default)!.Returns(ValueTask.FromResult<DeliveryRequest>(null!));

        // Act
        var action = () => _deliveryRequestService.DeleteAsync(deliveryRequestId, default);

        // Assert
        await action.Should().ThrowExactlyAsync<NullReferenceException>()
                    .WithMessage($"Attempt to delete nonexistent delivery request {deliveryRequestId}");
    }

    [Theory]
    [InlineData(DeliveryRequestStatus.Cancelled)]
    [InlineData(DeliveryRequestStatus.InProcess)]
    [InlineData(DeliveryRequestStatus.Done)]
    public async Task DeleteAsync_ShouldThrowsExceptionWhenRequestStatusIsNotNew(DeliveryRequestStatus status)
    {
        // Arrange
        var deliveryRequest = GetTestDeliveryRequest();
        deliveryRequest.Status = status;
        _deliveryRequestRepository.GetByIdAsync(deliveryRequest.Id, default)!.Returns(ValueTask.FromResult(deliveryRequest));

        // Act
        var action = () => _deliveryRequestService.DeleteAsync(deliveryRequest.Id, default);

        // Assert
        await action.Should().ThrowExactlyAsync<Exception>()
                    .WithMessage($"Attempt to delete delivery request {deliveryRequest.Id} which status is not New");
    }

    private DeliveryRequest GetTestDeliveryRequest() => new()
                                                        {
                                                            Id = Guid.NewGuid(),
                                                            DepartureAddress = "Test DA",
                                                            DepartureTime = DateTime.MaxValue,
                                                            DestinationAddress = "Test DA",
                                                            DestinationTime = DateTime.MaxValue,
                                                            ClientId = Guid.NewGuid(),
                                                            CargoId = Guid.NewGuid()
                                                        };
}
