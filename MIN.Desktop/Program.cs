using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<IKeyProvider, KeyProvider>();
            services.AddSingleton<ICryptoProvider, CryptoProvider>();

            services.AddSingleton<IPipeMessageSerializer, CryptoPipeMessageSerializer>();
            services.AddSingleton<ISettingsProvider, SettingsProvider>();
            services.AddSingleton<ILoggerProvider, LoggerProvider>();

            // ChatRoomService Ч синглтон на уровне приложени€ (один активный чат)
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