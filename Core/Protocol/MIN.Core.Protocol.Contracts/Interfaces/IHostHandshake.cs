using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Protocol.Contracts.Models;

namespace MIN.Core.Protocol.Contracts.Interfaces;

/// <summary>
/// Preamble-протокол со стороны хоста
/// </summary>
public interface IHostHandshake
{
    /// <summary>
    /// Выполнить handshake со стороны хоста
    /// </summary>
    Task<PreambleResult> HandleServerAsync(Guid serverConnectionId, Guid clientConnectionId, RoomInfo roomInfo, CancellationToken cancellationToken = default);
}
