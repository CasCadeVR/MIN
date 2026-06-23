using MIN.Core.Stores.Contracts.Models;

namespace MIN.Sessions.Core.Services.Contracts.Interfaces;

/// <summary>
/// Помошник в поиске сообщения, представляющее сессию
/// </summary>
public interface ISessionReadyMessageResolver
{
    /// <summary>
    /// Получить id сообщения, представляющее сессию по id подкомнаты
    /// </summary>
    Guid? GetSessionReadyMessageIdOutOfSubRoomId(RoomContext context, int subRoomId);
}
