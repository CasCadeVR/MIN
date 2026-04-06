using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Models;
using MIN.Helpers.Services;

namespace MIN.Desktop.Infrastructure.Bootstrap
{
    /// <summary>
    /// Bootstrap helper to create endpoints for rooms based on current transport configuration.
    /// </summary>
    public static class EndpointBootstrapper
    {
        /// <summary>
        /// Creates an endpoint for a given room using the current transport type.
        /// Right now supports NamedPipeEndpoint; in future can switch to TCP endpoint when implemented.
        /// </summary>
        public static IEndpoint CreateEndpointForRoom(Guid roomId)
        {
            var machineName = Environment.MachineName;
            var pipeName = PipeNameProvider.GetRoomPipeName(roomId);
            return new NamedPipeEndpoint(machineName, pipeName);
        }
    }
}
