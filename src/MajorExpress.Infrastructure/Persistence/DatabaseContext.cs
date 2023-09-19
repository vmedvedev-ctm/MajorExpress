using System.Reflection;

using MajorExpress.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace MajorExpress.Infrastructure.Persistence;

public class DatabaseContext : DbContext
{
    public DbSet<CancelComment> CancelComments { get; set; }

    public DbSet<Cargo> Cargoes { get; set; }

    public DbSet<Client> Clients { get; set; }

    public DbSet<DeliveryMan> DeliveryMen { get; set; }

    public DbSet<DeliveryRequest> DeliveryRequests { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
