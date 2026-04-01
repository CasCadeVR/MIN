using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
using MIN.Core.Cryptography;
using MIN.Core.Cryptography.Contracts.Interfaces.Storage;
using MIN.Core.Cryptography.Storage;
using MIN.Core.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Dispatcher;
using MIN.Core.Handlers.Handlers;
using MIN.Core.Messaging;
using MIN.Core.Serialization.Json;
using MIN.Core.Services;
using MIN.Core.Services.Messaging;
using MIN.Core.Services.Rooms;
using MIN.Core.Transport.NamedPipes;

namespace MIN.Core.DI;

/// <summary>
/// Модуль регистрации зависимостей для Core
/// </summary>
public class CoreModule : Module
{
    /// <inheritdoc />
    protected override void Load(IServiceCollection services)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var baseDirectory = Path.Combine(appData, "MIN-Messenger");
        services.AddSingleton<IKeyStorage>(sp => new FileSystemKeyStorage(baseDirectory));

        services.RegisterAsImplementedInterfaces<JsonDeserializerRegistry>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<JsonMessageSerializer>(ServiceLifetime.Singleton);

        services.RegisterAsImplementedInterfaces<MessageEncryptor>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<KeyProvider>(ServiceLifetime.Singleton);

        services.RegisterAsImplementedInterfaces<NamedPipeTransport>(ServiceLifetime.Singleton);

        services.RegisterAsImplementedInterfaces<ParticipantRegistry>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<MessageReceiver>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<MessageSender>(ServiceLifetime.Singleton);

        services.RegisterAsImplementedInterfaces<RoomConnector>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<ConnectionMonitor>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<RoomRegistry>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<RoomHoster>(ServiceLifetime.Singleton);

        services.RegisterMultipleInterfacesAssignableTo<IMessageHandler, ICoreHandlerAnchor>(ServiceLifetime.Singleton);

        services.RegisterAsImplementedInterfaces<InMemoryEventBus>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<MessageDispatcher>(ServiceLifetime.Singleton);

        services.RegisterMessagesFromAnchor<ICoreMessagingAnchor>();
    }
}
