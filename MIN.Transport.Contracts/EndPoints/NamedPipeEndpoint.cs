namespace MIN.Transport.Contracts.Endpoints;

/// <summary>
/// Точка подключения для Named Pipes
/// </summary>
public sealed record NamedPipeEndpoint : IEndpoint
{
    public TransportType Type => TransportType.NamedPipe;

    /// <summary>
    /// Имя машины
    /// </summary>
    public string MachineName { get; init; }

    /// <summary>
    /// Имя именованного канала
    /// </summary>
    public string PipeName { get; init; }

    public NamedPipeEndpoint(string machineName, string pipeName)
    {
        MachineName = machineName;
        PipeName = pipeName;
    }
}
