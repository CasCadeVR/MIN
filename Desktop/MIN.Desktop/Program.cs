using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Common.Mvc.Extensions;
using MIN.Desktop.Infrastructure.Services;
using MIN.DI;

namespace MIN.Desktop
{
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

            var serviceProvider = services.BuildServiceProvider();

            using var appLifeTimeCts = new CancellationTokenSource();

            foreach (var hostedService in serviceProvider.GetServices<IHostedService>())
            {
                await hostedService.StartAsync(appLifeTimeCts.Token);
            }

            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            mainForm.FormClosing += (sender, e) =>
            {
                appLifeTimeCts.Cancel();
            };

            Application.Run(mainForm);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var appVersion = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(0, 0, 0, 0);
            services.AddSingleton(appVersion);

            services.RegisterAsImplementedInterfaces<NotificationService>(ServiceLifetime.Singleton);
            services.RegisterAsImplementedInterfaces<SettingsProvider>(ServiceLifetime.Singleton);
            services.RegisterModule<MinModule>();

            services.AddScoped<MainForm>();
        }
    }
}
