namespace MajorExpress.Domain.Entities.Base;

public class DeliveryRequestHolderBase : EntityBase
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public ICollection<DeliveryRequest> DeliveryRequests { get; set; } = new List<DeliveryRequest>();

    protected DeliveryRequestHolderBase()
    {
    }
}
