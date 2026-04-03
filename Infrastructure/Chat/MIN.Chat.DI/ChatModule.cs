using Microsoft.Extensions.DependencyInjection;
using MIN.Chat.Handlers;
using MIN.Chat.Messaging;
using MIN.Chat.Services;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
using MIN.Core.Handlers.Contracts;

namespace MIN.Chat.DI;

/// <summary>
/// Модуль регистрации зависимостей для Chat
/// </summary>
public class ChatModule : Module
{
    /// <inheritdoc />
    protected override void Load(IServiceCollection services)
    {
        services.RegisterMultipleMessagesFromAnchor<IChatMessagingAnchor>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<ChatService>(ServiceLifetime.Singleton);
        services.RegisterMultipleInterfacesAssignableTo<IMessageHandler, IChatHandlerAnchor>(ServiceLifetime.Singleton);
    }
}
