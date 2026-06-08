using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
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
        services.RegisterMultipleInterfacesAssignableTo<ISessionPresenter, ChessSessionPresenter>(ServiceLifetime.Singleton);
    }
}
