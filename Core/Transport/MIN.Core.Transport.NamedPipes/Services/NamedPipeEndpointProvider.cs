using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Models;

namespace MIN.Core.Transport.NamedPipes.Services;

/// <inheritdoc cref="IEndPointProvider"/>
public class NamedPipeEndpointProvider : IEndPointProvider
{
    IEndpoint IEndPointProvider.CreateEndpointForRoom(Guid roomId)
    {
        var machineName = Environment.MachineName;
        var pipeName = PipeNameProvider.GetRoomPipeName(roomId);
        return new NamedPipeEndpoint(machineName, pipeName);
    }
}
