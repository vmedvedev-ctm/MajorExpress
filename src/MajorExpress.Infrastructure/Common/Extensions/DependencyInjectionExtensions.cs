using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MajorExpress.Infrastructure.Common.Extensions;

internal static class DependencyInjectionExtensions
{
    internal static IServiceCollection ConfigureOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class
    {
        services.AddOptions<TOptions>()
                .Bind(configuration.GetSection(typeof(TOptions).Name))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        return services;
    }
}
