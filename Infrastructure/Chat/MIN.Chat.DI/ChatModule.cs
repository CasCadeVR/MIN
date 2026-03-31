using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
using MIN.Chat.Services;
using MIN.Chat.Handlers;
using MIN.Chat.Messaging;

namespace MIN.Chat.DI;

/// <summary>
/// Модуль регистрации зависимостей для Chat
/// </summary>
public class ChatModule : Module
{
    /// <inheritdoc />
    protected override void Load(IServiceCollection services)
    {
        services.RegisterMessagesFromAnchor<IChatMessagingAnchor>();
        services.RegisterAsImplementedInterfaces<ChatService>(ServiceLifetime.Singleton);
        services.RegisterAssemblyInterfacesAssignableTo<IChatHandlerAnchor>(ServiceLifetime.Singleton);
    }
}
