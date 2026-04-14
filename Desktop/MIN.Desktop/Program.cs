using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MIN.Common.Mvc.Extensions;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
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

            var messageReceiver = serviceProvider.GetRequiredService<IMessageReceiver>();
            await messageReceiver.StartListeningAsync(appLifeTimeCts.Token);

            var connectionMonitor = serviceProvider.GetRequiredService<IConnectionMonitor>();
            await connectionMonitor.StartAsync(appLifeTimeCts.Token);

            var jsonDeserializerInitializer = serviceProvider.GetRequiredService<IHostedService>();
            await jsonDeserializerInitializer.StartAsync(appLifeTimeCts.Token);

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


            var projectDir = Path.GetDirectoryName(AppContext.BaseDirectory)!;
            while (!File.Exists(Path.Combine(projectDir, "MIN.Desktop.csproj")))
            {
                projectDir = Path.GetDirectoryName(projectDir)!;
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(projectDir)
                .AddJsonFile("appsettings.json", optional: false).Build();

            services.AddSingleton<IConfiguration>(config);

            services.RegisterAsImplementedInterfaces<NotificationService>(ServiceLifetime.Singleton);
            services.RegisterAsImplementedInterfaces<SettingsProvider>(ServiceLifetime.Singleton);
            services.RegisterModule<MinModule>();

            services.AddScoped<MainForm>();
        }
    }
}
