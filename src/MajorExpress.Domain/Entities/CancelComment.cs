using MajorExpress.Domain.Entities.Base;

namespace MajorExpress.Domain.Entities;

public class CancelComment : EntityBase
{
    public Guid RequestId { get; set; }

    public DeliveryRequest DeliveryRequest { get; set; } = default!;

    public string Comment { get; set; } = default!;
}
