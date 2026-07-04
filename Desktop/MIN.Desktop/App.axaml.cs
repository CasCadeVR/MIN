using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Infrastructure.Extensions;

namespace MIN.Desktop;

/// <summary>
/// Приложение
/// </summary>
public partial class App : Application
{
    static internal Func<Window> StartupWindowFactory = null!;

    /// <summary>
    /// Экземпляр класса
    /// </summary>
    public static App Instance = null!;

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
            var hostedServices = serviceProvider.GetServices<IHostedService>();

            foreach (var hostedService in hostedServices)
            {
                Task.Run(() => hostedService.StartAsync(appLifeTimeCts.Token));
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
        Instance = this;

        ApplyAppDefaults();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = StartupWindowFactory();
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <inheritdoc />
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    private void ApplyAppDefaults()
    {
        RequestedThemeVariant = ThemeVariant.Dark;
    }
}
