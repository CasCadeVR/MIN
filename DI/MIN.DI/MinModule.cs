using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MIN.Chat.DI;
using MIN.Common.Mvc.Extensions;
using MIN.Core.DI;
using MIN.DI.FeatureCollection;
using MIN.Discovery.DI;
using MIN.FileTransfer.DI;
using MIN.Helpers.DI;

namespace MIN.DI;

/// <summary>
/// Модуль регистрации зависимостей для MIN
/// </summary>
public class MinModule : Common.Mvc.Module
{
    /// <inheritdoc />
    protected override void Load(IServiceCollection services)
    {
        var appVersion = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(0, 0, 0, 0);
        services.AddSingleton(appVersion);

        services.RegisterModule<HelpersModule>();
        services.RegisterModule<CoreModule>();
        services.RegisterModule<ChatModule>();
        services.RegisterModule<FileTransferModule>();
        services.RegisterModule<DiscoveryModule>();

        services.RegisterAsImplementedInterfaces<MinFeatureCollection>(ServiceLifetime.Singleton);
    }
}
