using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Helpers.Services;

namespace MIN.Desktop.Infrastructure.Bootstrap
{
    /// <summary>
    /// Помощник Bootstrap для создания конечных точек для комнат на основе текущей конфигурации транспорта.
    /// </summary>
    public static class EndpointBootstrapper
    {
        /// <summary>
        /// оздает конечную точку для данной комнаты, используя текущий тип транспорта.
        /// </summary>
        public static IEndpoint CreateEndpointForRoom(Guid roomId)
        {
            var machineName = Environment.MachineName;
            var pipeName = PipeNameProvider.GetRoomPipeName(roomId);
            return new NamedPipeEndpoint(machineName, pipeName);
        }
    }
}
