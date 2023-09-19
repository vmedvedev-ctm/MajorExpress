using MajorExpress.Application.Common.Interfaces;
using MajorExpress.Application.Common.Interfaces.Repository;
using MajorExpress.Domain.Entities;

using Microsoft.AspNetCore.Mvc;

namespace MajorExpress.WebApi.Properties;

[Route("api/[Controller]/[action]")]
[ApiController]
public class DeliveryRequestController : ControllerBase
{
    private readonly IDeliveryRequestService _deliveryRequestService;

    private readonly IDeliveryRequestRepository _deliveryRequestRepository;

    public DeliveryRequestController(IDeliveryRequestService deliveryRequestService, IDeliveryRequestRepository deliveryRequestRepository)
    {
        _deliveryRequestService = deliveryRequestService;
        _deliveryRequestRepository = deliveryRequestRepository;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), 200)]
    public async Task<IActionResult> CreateRequestAsync(
        string departureAddress,
        DateTime departureTime,
        string destinationAddress,
        DateTime destinationTime,
        Guid clientId,
        Guid cargoId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _deliveryRequestService.CreateAsync(
                             departureAddress,
                             departureTime,
                             destinationAddress,
                             destinationTime,
                             clientId,
                             cargoId,
                             cancellationToken);

            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AssignAsync(Guid deliveryRequestId, Guid assigneeId, CancellationToken cancellationToken)
    {
        try
        {
            await _deliveryRequestService.AssignAsync(deliveryRequestId, assigneeId, cancellationToken);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CancelAsync(Guid deliveryRequestId, string comment, CancellationToken cancellationToken)
    {
        try
        {
            await _deliveryRequestService.CancelAsync(deliveryRequestId, comment, cancellationToken);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<DeliveryRequest>), 200)]
    public async Task<IActionResult> SearchAsync(string tag, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _deliveryRequestService.SearchAsync(tag, cancellationToken);
            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CompleteAsync(Guid deliveryRequestId, CancellationToken cancellationToken)
    {
        try
        {
            await _deliveryRequestService.CompleteAsync(deliveryRequestId, cancellationToken);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e);
        }
    }

    [HttpPatch]
    public async Task<IActionResult> EditAsync(
        Guid deliveryRequestId,
        CancellationToken cancellationToken,
        string? departureAddress = null,
        DateTime? departureTime = null,
        string? destinationAddress = null,
        DateTime? destinationTime = null,
        Guid? clientId = null,
        Guid? cargoId = null)
    {
        try
        {
            await _deliveryRequestService.EditAsync(
                deliveryRequestId,
                cancellationToken,
                departureAddress,
                departureTime,
                destinationAddress,
                destinationTime,
                clientId,
                cargoId);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(Guid deliveryRequestId, CancellationToken cancellationToken)
    {
        try
        {
            await _deliveryRequestService.DeleteAsync(deliveryRequestId, cancellationToken);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<DeliveryRequest>), 200)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _deliveryRequestRepository.GetNoTrackingListAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e);
        }
    }
}
