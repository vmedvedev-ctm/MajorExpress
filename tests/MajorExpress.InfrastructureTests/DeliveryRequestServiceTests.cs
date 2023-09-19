using FluentAssertions;

using MajorExpress.Application.Common.Interfaces;
using MajorExpress.Application.Common.Interfaces.Repository;
using MajorExpress.Domain.Entities;
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
        var deliveryRequest = new DeliveryRequest()
                              {
                                  Id = Guid.NewGuid(),
                                  DepartureAddress = "Test DA",
                                  DepartureTime = DateTime.MaxValue,
                                  DestinationAddress = "Test DA",
                                  DestinationTime = DateTime.MaxValue,
                                  ClientId = Guid.NewGuid(),
                                  CargoId = Guid.NewGuid()
                              };

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
}
