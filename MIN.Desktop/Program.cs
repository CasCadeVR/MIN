using Microsoft.Extensions.DependencyInjection;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Infrastructure.Services;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Connection.Contracts.Interfaces.Discovering;
using MIN.Services.Connection.Contracts.Interfaces.Pipes;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Connection.Cryptographing;
using MIN.Services.Connection.Pipes;
using MIN.Services.Connection.Pipes.Discovering;
using MIN.Services.Connection.Serialize;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Services;
using System.Reflection;

namespace MIN.Desktop
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            Application.Run(serviceProvider.GetRequiredService<MainForm>());
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var appVersion = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(0, 0, 0, 0);
            services.AddSingleton(appVersion);

            services.AddSingleton<ILoggerProvider, LoggerProvider>();

            services.AddSingleton<IKeyProvider, KeyProvider>();
            services.AddSingleton<ICryptoProvider, CryptoProvider>();
            services.AddSingleton<IUpdateService, GitHubUpdateService>();
            services.AddSingleton<INotificationService, NotificationService>();

            services.AddSingleton<IPipeMessageSerializer, CommonPipeMessageSerializer>();
            services.AddSingleton<ISettingsProvider, SettingsProvider>();
            services.AddSingleton<IChatRoomService, ChatRoomService>();

            services.AddTransient<IPipeRoomServer, PipeRoomServer>();
            services.AddTransient<IPipeParticipantClient, PipeParticipantClient>();

            services.AddTransient<IDiscoveryServer, DiscoveryServer>();
            services.AddTransient<IDiscoveryClient, DiscoveryClient>();

            services.AddScoped<MainForm>();
            services.AddScoped<RoomCreateForm>();
        }
    }
}