using MajorExpress.Application.Common.Interfaces;
using MajorExpress.Application.Common.Interfaces.Repository;
using MajorExpress.Infrastructure.Common.Extensions;
using MajorExpress.Infrastructure.Options;
using MajorExpress.Infrastructure.Persistence;
using MajorExpress.Infrastructure.Persistence.Repository;
using MajorExpress.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

namespace MajorExpress.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSettings(configuration)
                .AddDatabase(configuration)
                .AddRepositories()
                .AddServices();

    private static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services.AddSingleton<ICancelCommentRepository, CancelCommentRepository>()
                .AddSingleton<IDeliveryRequestRepository, DeliveryRequestRepository>();

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(nameof(PostgresOptions)).Get<PostgresOptions>();

        var npgsqlBuilder = new NpgsqlDataSourceBuilder(options?.ConnectionString);

        return services.AddDbContextFactory<DatabaseContext>(o => o.UseNpgsql(npgsqlBuilder.Build()));
    }

    private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration) =>
        services.ConfigureOptions<PostgresOptions>(configuration);

    private static IServiceCollection AddServices(this IServiceCollection services) =>
        services.AddSingleton<IDeliveryRequestService, DeliveryRequestService>();
}
