using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;

namespace MIN.FileTransfer.DI;

/// <summary>
/// Модуль регистрации зависимостей для Chat
/// </summary>
public class FileTransferModule : Module
{
    /// <inheritdoc />
    protected override void Load(IServiceCollection services)
    {
        services.RegisterMultipleInterfacesAssignableFromAnchor<IMessage, IChatMessagingAnchor>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<ChatService>(ServiceLifetime.Singleton);
        services.RegisterMultipleInterfacesAssignableFromAnchor<IMessageHandler, IChatHandlerAnchor>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<ChatFeatureCollection>(ServiceLifetime.Singleton);
    }
}
