using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
using MIN.Core.Cryptography;
using MIN.Core.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Dispatcher;
using MIN.Core.Handlers.Handlers;
using MIN.Core.Messaging;
using MIN.Core.Serialization.Json;
using MIN.Core.Serialization.Json.Services;
using MIN.Core.Services.Messaging;
using MIN.Core.Services.Rooms;
using MIN.Core.Stores.Registries;
using MIN.Core.Stores.Services;
using MIN.Core.Streaming.Services;
using MIN.Core.Transport.NamedPipes;
using MIN.Core.Transport.NamedPipes.Factories;
using MIN.Core.Transport.NamedPipes.Services;

namespace MIN.Core.DI;

/// <summary>
/// Модуль регистрации зависимостей для Core
/// </summary>
public class CoreModule : Module
{
    /// <inheritdoc />
    protected override void Load(IServiceCollection services)
    {
        services.RegisterAsImplementedInterfaces<JsonDeserializerRegistry>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<JsonMessageSerializer>(ServiceLifetime.Singleton);
        services.AddHostedService<JsonDeserializerInitializer>();

        services.RegisterAsImplementedInterfaces<MessageEncryptor>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<FileSystemKeyStorage>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<KeyProvider>(ServiceLifetime.Singleton);

        services.RegisterAsImplementedInterfaces<NamedPipeEndpointProvider>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<NamedPipeTransport>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<NamedPipeTransportFactory>(ServiceLifetime.Singleton);

        services.RegisterAsImplementedInterfaces<ParticipantConnectionRegistry>(ServiceLifetime.Singleton);

        services.RegisterAsImplementedInterfaces<MessageStore>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<ParticipantStore>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<RoomStore>(ServiceLifetime.Singleton);

        services.RegisterAsImplementedInterfaces<MessageReceiver>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<MessageSender>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<MessageRouter>(ServiceLifetime.Singleton);

        services.RegisterAsImplementedInterfaces<ChunkBufferAssembler>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<StreamManager>(ServiceLifetime.Singleton);

        services.RegisterAsImplementedInterfaces<RoomConnector>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<ConnectionMonitor>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<RoomHoster>(ServiceLifetime.Singleton);

        services.RegisterMultipleInterfacesAssignableTo<IMessageHandler, ICoreHandlerAnchor>(ServiceLifetime.Singleton);

        services.RegisterAsImplementedInterfaces<InMemoryEventBus>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<MessageDispatcher>(ServiceLifetime.Singleton);

        services.RegisterMultipleMessagesFromAnchor<ICoreMessagingAnchor>(ServiceLifetime.Singleton);
    }
}
