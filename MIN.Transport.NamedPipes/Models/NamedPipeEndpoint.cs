using MIN.Transport.Contracts.Enum;
using MIN.Transport.Contracts.Models;

namespace MIN.Transport.NamedPipes.Models;

/// <summary>
/// Точка подключения для Named Pipes
/// </summary>
internal sealed record NamedPipeEndpoint : IEndpoint
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
}
