using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
using MIN.Discovery.Services;
using MIN.Discovery.Transport.NamedPipes;

namespace MIN.Discovery.DI;

/// <summary>
/// Модуль регистрации зависимостей для Discovery
/// </summary>
public class DiscoveryModule : Module
{
    /// <inheritdoc />
    protected override void Load(IServiceCollection services)
    {
        services.RegisterAsImplementedInterfaces<NamedPipeDiscoveryTransport>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<DiscoveryService>(ServiceLifetime.Singleton);
    }
}
