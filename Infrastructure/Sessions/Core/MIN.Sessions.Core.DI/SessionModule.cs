using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Rooms;
using MIN.Sessions.Core.DI.FeatureCollection;
using MIN.Sessions.Core.Handlers;
using MIN.Sessions.Core.Messaging;
using MIN.Sessions.Core.Services;

namespace MIN.Sessions.Core.DI;

/// <summary>
/// Модуль регистрации зависимостей для Chat
/// </summary>
public class SessionModule : Module
{
    /// <inheritdoc />
    protected override void Load(IServiceCollection services)
    {
        services.RegisterMultipleInterfacesAssignableFromAnchor<IMessage, ISessionsMessagingAnchor>(ServiceLifetime.Singleton);
        services.RegisterMultipleInterfacesAssignableFromAnchor<IMessageHandler, ISessionsHandlerAnchor>(ServiceLifetime.Singleton);
        services.RegisterMultipleInterfacesAssignableTo<IHostedService, SessionMonitor>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<SessionFeatureCollection>(ServiceLifetime.Singleton);
    }
}
