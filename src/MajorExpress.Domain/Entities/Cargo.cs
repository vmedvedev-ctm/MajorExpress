using MajorExpress.Domain.Entities.Base;

namespace MajorExpress.Domain.Entities;

public class Cargo : EntityBase
{
    public string Name { get; set; } = default!;

    public decimal Weight { get; set; }

    public decimal Height { get; set; }

    public decimal Width { get; set; }

    public decimal Lenght { get; set; }
}
