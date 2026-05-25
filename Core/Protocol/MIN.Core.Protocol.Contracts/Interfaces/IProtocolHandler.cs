using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Protocol.Contracts.Models;

namespace MIN.Core.Protocol.Contracts.Interfaces;

/// <summary>
/// Обработчик протокола общения
/// </summary>
public interface IProtocolHandler
{
    /// <summary>
    /// Обработать сервер
    /// </summary>
    Task<PreambleResult> HandleServerAsync(Guid serverConnectionId, Guid clientConnectionId, RoomInfo roomInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обработать клиента
    /// </summary>
    Task<PreambleResult> HandleClientAsync(Guid connectionId, CancellationToken cancellationToken = default);
}
