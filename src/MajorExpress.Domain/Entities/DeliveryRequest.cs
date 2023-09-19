using MajorExpress.Domain.Entities.Base;
using MajorExpress.Domain.Enums;

namespace MajorExpress.Domain.Entities;

public class DeliveryRequest : EntityBase
{
    public string DepartureAddress { get; set; } = default!;

    public DateTime DepartureTime { get; set; }

    public string DestinationAddress { get; set; } = default!;

    public DateTime DestinationTime { get; set; }

    public DeliveryRequestStatus Status { get; set; } = DeliveryRequestStatus.New;

    public Guid? AssigneeId { get; set; }

    public DeliveryMan? Assignee { get; set; }

    public Guid ClientId { get; set; }

    public Client Client { get; set; } = default!;

    public Guid CargoId { get; set; }

    public Cargo Cargo { get; set; } = default!;
}
