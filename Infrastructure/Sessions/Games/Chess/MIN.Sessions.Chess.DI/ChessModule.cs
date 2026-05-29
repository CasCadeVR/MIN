using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Sessions.Chess.Messaging;
using MIN.Sessions.Chess.Services.Services;
using MIN.Sessions.Core.Services.Contracts.Interfaces;

namespace MIN.Sessions.Chess.DI;

/// <summary>
/// Модуль регистрации зависимостей для Chess
/// </summary>
public class ChessModule : Module
{
    /// <inheritdoc />
    protected override void Load(IServiceCollection services)
    {
        services.RegisterMultipleInterfacesAssignableFromAnchor<IMessage, IChessMessagingAnchor>(ServiceLifetime.Singleton);
        services.RegisterMultipleInterfacesAssignableTo<ISessionPresenter, ChessSessionPresenter>(ServiceLifetime.Singleton);
    }
}
