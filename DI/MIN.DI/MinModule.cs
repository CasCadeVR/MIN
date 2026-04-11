using Microsoft.Extensions.DependencyInjection;
using MIN.Chat.DI;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
using MIN.Core.DI;
using MIN.Discovery.DI;
using MIN.Helpers.DI;

namespace MIN.DI;

/// <summary>
/// Модуль регистрации зависимостей для MIN
/// </summary>
public class MinModule : Module
{
    /// <inheritdoc />
    protected override void Load(IServiceCollection services)
    {
        services.RegisterModule<HelpersModule>();
        services.RegisterModule<CoreModule>();
        services.RegisterModule<ChatModule>();
        services.RegisterModule<DiscoveryModule>();
    }
}
