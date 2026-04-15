using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
using MIN.Helpers.Services;

namespace MIN.Helpers.DI;

/// <summary>
/// Модуль регистрации зависимостей для Helpers
/// </summary>
public class HelpersModule : Module
{
    /// <inheritdoc />
    protected override void Load(IServiceCollection services)
    {
        services.RegisterAsImplementedInterfaces<LoggerProvider>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<IdentityService>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<AppDataProvider>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<FileSystemSettingsStorage>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<CollegeNetworkComputerProvider>(ServiceLifetime.Singleton);
    }
}
