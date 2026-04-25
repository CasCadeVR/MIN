using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Common.Mvc.Extensions;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Views.Panels.PanelViews;
using MIN.Desktop.Views.Panels.SidePanelViews;
using MIN.DI;

namespace MIN.Desktop;

/// <summary>
/// ¬ходна€ точка программы
/// </summary>
static internal class Program
{
    /// <summary>
    /// ¬ходной метод программы
    /// </summary>
    [STAThread]
    public static async Task Main()
    {
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        ConfigureServices(services);
        ConfigurePanels(services);

        var serviceProvider = services.BuildServiceProvider();

        var appLifeTimeCts = serviceProvider.GetRequiredService<ICtsProvider>().AppCts;

        foreach (var hostedService in serviceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(appLifeTimeCts.Token);
        }

        var mainForm = serviceProvider.GetRequiredService<MainForm>();
        mainForm.FormClosing += (sender, e) =>
        {
            appLifeTimeCts.Cancel();
            appLifeTimeCts.Dispose();
        };

        Application.Run(mainForm);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.RegisterAsImplementedInterfaces<CtsProvider>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<NavigationService>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<NotificationService>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<SettingsProvider>(ServiceLifetime.Singleton);
        services.RegisterModule<MinModule>();

        services.RegisterAsImplementedInterfaces<MainForm>(ServiceLifetime.Transient);
    }

    private static void ConfigurePanels(IServiceCollection services)
    {
        services.RegisterAsImplementedInterfaces<MainSidePanelView>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<SettingsSidePanelView>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<DiscoveryPanelView>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<ChatPanelView>(ServiceLifetime.Transient);
    }
}
