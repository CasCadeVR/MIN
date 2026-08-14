using MIN.Sessions.Core.Transport.Contracts.Interfaces;
using MIN.Sessions.Core.Transport.NamedPipes;
using MIN.Sessions.Core.Transport.Tcp;

namespace MIN.Sessions.Core.Transport;

/// <inheritdoc cref="ISessionTransportFactory"/>
public class SessionTransportFactory : ISessionTransportFactory
{
    ISessionProcessTransport ISessionTransportFactory.Create()
        => OperatingSystem.IsWindows()
            ? new NamedPipeProcessTransport()
            : new TcpLoopbackTransport();

    async void ISessionTransportFactory.Destroy(ISessionProcessTransport transport)
    {
        await transport.StopAsync();
        await transport.DisposeAsync();
    }
}
