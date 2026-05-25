using MIN.Core.Transport.Contracts.Enum;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Core.Transport.TcpSockets.Models;

/// <summary>
/// Точка подключения к комнате на основе TCP Sockets
/// </summary>
public sealed class TcpEndpoint : IEndpoint
{
    /// <inheritdoc />
    public TransportType Type => TransportType.Tcp;

    /// <summary>
    /// IP Адрес
    /// </summary>
    public string IPAddress { get; set; } = string.Empty;

    /// <summary>
    /// Динамически созданный порт
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="TcpEndpoint"/>
    /// </summary>
    public TcpEndpoint() { }

    /// <inheritdoc />
    public override string ToString() => $"{IPAddress}:{Port}";
}
