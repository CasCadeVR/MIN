using MIN.Core.Entities;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Transport.Contracts.Interfaces;

namespace MIN.Services.Contracts.Interfaces;

/// <summary>
/// Сервис для обработок подключения и создания комнат
/// </summary>
public interface IRoomService
{
    /// <summary>
    /// Получить информацию комнате
    /// </summary>
    Room GetRoom(Guid roomId);

    /// <summary>
    /// Ассоциирует участника с идентификатором соединения
    /// </summary>
    void SetRoom(Guid connectionId, Room room);

    /// <summary>
    /// Добавить участника в комнату
    /// </summary>
    void AddParticipant(Guid roomId, ParticipantInfo participant);

    /// <summary>
    /// Удалить участника из комнаты
    /// </summary>
    void RemoveParticipant(Guid roomId, Guid participantId);

    /// <summary>
    /// Запустить сервер подключений для указанной комнаты (режим сервера)
    /// </summary>
    Task StartHostingAsync(Guid roomId, IEndpoint endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Прекратить сервер для указанной комнаты
    /// </summary>
    Task StopHostingAsync(Guid roomId);

    /// <summary>
    /// Подключиться к удалённой комнате (режим клиента)
    /// </summary>
    Task<Guid> ConnectAsync(Guid roomId, IEndpoint endpoint, int timeoutMs = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отключиться от указанной комнаты
    /// </summary>
    Task DisconnectAsync(Guid roomId, Guid connectionId);
}
