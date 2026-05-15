using MIN.Core.Entities.Contracts.Enums;

namespace MIN.Chat.Services.Contracts.Interfaces;

/// <summary>
/// Сервис для работы с статусом действия в сети
/// </summary>
public interface IChatStatusService
{
    /// <summary>
    /// Отправить событие о смене своего статуса
    /// </summary>
    Task SendSelfOnlineStatusChangedAsync(Guid roomId, OnlineStatus newStatus, CancellationToken cancellationToken = default);
}
