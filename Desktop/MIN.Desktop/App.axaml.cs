using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Infrastructure.Extensions;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Desktop;

/// <summary>
/// Приложение
/// </summary>
public partial class App : Application
{
    static internal Func<Window> StartupWindowFactory = null!;

    /// <summary>
    /// Создать приложение
    /// </summary>
    public static AppBuilder Create()
    {
        StartupWindowFactory = () =>
        {
            var serviceProvider = new ServiceCollection()
                .AddAppServices()
                .BuildServiceProvider();

            var appLifeTimeCts = serviceProvider.GetRequiredService<ICtsProvider>().AppCts;
            var logger = serviceProvider.GetRequiredService<ILoggerProvider>();
            var hostedServices = serviceProvider.GetServices<IHostedService>();

            foreach (var hostedService in hostedServices)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await hostedService.StartAsync(appLifeTimeCts.Token);
                    }
                    catch (Exception ex)
                    {
                        logger.Log(ex.Message, Helpers.Contracts.Models.Enums.LogLevel.Error);
                    }
                });
            }

            return serviceProvider.GetRequiredService<Func<Window>>()();
        };

        return AppBuilder.Configure<App>()
                        .UsePlatformDetect()
#if DEBUG
                        .WithDeveloperTools()
#endif
                        .WithInterFont()
                        .LogToTrace()
                        .With(new SkiaOptions { UseOpacitySaveLayer = true });
    }

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = StartupWindowFactory();
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <inheritdoc />
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}
