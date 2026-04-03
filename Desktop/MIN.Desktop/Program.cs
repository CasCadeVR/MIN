using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MIN.Chat.DI;
using MIN.Common.Mvc.Extensions;
using MIN.Core.DI;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Desktop.Infrastructure.Services;
using MIN.Discovery.DI;
using MIN.Helpers.DI;

namespace MIN.Desktop
{
    static internal class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            using var cts = new CancellationTokenSource();

            var messageReceiver = serviceProvider.GetRequiredService<IMessageReceiver>();
            await messageReceiver.StartListeningAsync(cts.Token);

            var jsonDeserializerInitializer = serviceProvider.GetRequiredService<IHostedService>();
            await jsonDeserializerInitializer.StartAsync(cts.Token);

            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            mainForm.FormClosing += (sender, e) =>
            {
                cts.Cancel();
            };

            Application.Run(mainForm);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var appVersion = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(0, 0, 0, 0);
            services.AddSingleton(appVersion);

            services.RegisterAsImplementedInterfaces<NotificationService>(ServiceLifetime.Singleton);
            services.RegisterAsImplementedInterfaces<SettingsProvider>(ServiceLifetime.Singleton);

            services.RegisterModule<HelpersModule>();
            services.RegisterModule<CoreModule>();
            services.RegisterModule<ChatModule>();
            services.RegisterModule<DiscoveryModule>();

            services.AddScoped<MainForm>();
        }
    }
}
