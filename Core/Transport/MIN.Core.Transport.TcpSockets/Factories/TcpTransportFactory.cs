using MIN.Core.Transport.Contracts.Enum;
using MIN.Core.Transport.Contracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MIN.Core.Transport.TcpSockets.Factories;

/// <summary>
/// Фабрика для транспорта
/// </summary>
public class TcpTransportFactory : ITransportFactory
{
    private readonly IServiceProvider provider;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="TcpTransportFactory"/>
    /// </summary>
    public TcpTransportFactory(IServiceProvider provider)
    {
        this.provider = provider;
    }

    ITransport ITransportFactory.CreateTransport(TransportType type)
    {
        if (type != TransportType.Tcp)
        {
            throw new NotSupportedException($"Transport type '{type}' is not supported. Only TCP.");
        }
        return provider.GetRequiredService<TcpTransport>();
    }
}
