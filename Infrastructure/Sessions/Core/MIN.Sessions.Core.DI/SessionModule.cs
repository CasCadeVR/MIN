using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Sessions.Core.Messaging;

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
    }
}
