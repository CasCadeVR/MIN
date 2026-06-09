using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Common.Mvc.Extensions;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Views.Forms;
using MIN.Desktop.Views.Panels.PanelViews.ChatPanel;
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
    public static void Main()
    {
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        ConfigureServices(services);
        ConfigurePanels(services);

        var serviceProvider = services.BuildServiceProvider();

        var appLifeTimeCts = serviceProvider.GetRequiredService<ICtsProvider>().AppCts;

        var hostedServices = serviceProvider.GetServices<IHostedService>();

        foreach (var hostedService in hostedServices)
        {
            Task.Run(() => hostedService.StartAsync(appLifeTimeCts.Token));
        }

        var mainForm = serviceProvider.GetRequiredService<MainForm>();
        mainForm.FormClosing += (sender, e) =>
        {
            appLifeTimeCts.Cancel();
            appLifeTimeCts.Dispose();

            foreach (var hostedService in hostedServices)
            {
                Task.Run(() => hostedService.StopAsync());
            }
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
    }

    private static void ConfigurePanels(IServiceCollection services)
    {
        services.RegisterAsImplementedInterfaces<MainForm>(ServiceLifetime.Transient);

        services.RegisterAsImplementedInterfaces<MainSidePanelView>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<SettingsSidePanelView>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<DiscoveryPanelView>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<ChatPanelView>(ServiceLifetime.Transient);
    }
}
