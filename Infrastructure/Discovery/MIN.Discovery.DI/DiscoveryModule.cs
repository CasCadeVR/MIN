using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Discovery.DI.FeatureCollection;
using MIN.Discovery.Messaging;
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
        services.RegisterMultipleInterfacesAssignableFromAnchor<IMessage, IDiscoveryMessagingAnchor>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<NamedPipeDiscoveryTransport>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<DiscoveryService>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<DiscoveryFeatureCollection>(ServiceLifetime.Singleton);
    }
}
