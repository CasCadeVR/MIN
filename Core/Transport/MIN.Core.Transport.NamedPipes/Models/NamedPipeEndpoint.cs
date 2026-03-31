using MIN.Core.Transport.Contracts.Enum;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Core.Transport.NamedPipes.Models;

/// <summary>
/// Точка подключения для Named Pipes
/// </summary>
public sealed record NamedPipeEndpoint : IEndpoint
{
    /// <inheritdoc />
    public TransportType Type => TransportType.NamedPipe;

    /// <summary>
    /// Имя машины
    /// </summary>
    public string MachineName { get; set; } = string.Empty;

    /// <summary>
    /// Имя именованного канала
    /// </summary>
    public string PipeName { get; set; } = string.Empty;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NamedPipeEndpoint"/>
    /// </summary>
    public NamedPipeEndpoint(string machineName, string pipeName)
    {
        MachineName = machineName;
        PipeName = pipeName;
    }
}
