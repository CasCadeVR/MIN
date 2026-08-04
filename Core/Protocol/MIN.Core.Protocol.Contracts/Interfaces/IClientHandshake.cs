using MIN.Core.Protocol.Contracts.Models;

namespace MIN.Core.Protocol.Contracts.Interfaces;

/// <summary>
/// Preamble-протокол со стороны клиента (подключающегося)
/// </summary>
public interface IClientHandshake
{
    /// <summary>
    /// Выполнить handshake со стороны клиента
    /// </summary>
    Task<PreambleResult> HandleClientAsync(Guid connectionId, CancellationToken cancellationToken = default);
}
