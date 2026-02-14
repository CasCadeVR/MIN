using Microsoft.Extensions.DependencyInjection;
using MIN.Services.Connection.Contracts.Interfaces;
using MIN.Services.Connection.Pipes;
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
            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IRoomService, RoomService>();

            services.AddTransient<IPipeRoomServer, PipeRoomServer>();
            services.AddTransient<IPipeParticipantClient, PipeParticipantClient>();

            // —ериализатор Ч стейтлесс, можно синглтон
            services.AddSingleton<PipeMessageSerializer>();

            // ChatRoomService Ч синглтон на уровне приложени€ (один активный чат)
            services.AddSingleton<IChatRoomService, ChatRoomService>();

            services.AddScoped<MainForm>();
            services.AddScoped<RoomCreateForm>();
        }
    }
}