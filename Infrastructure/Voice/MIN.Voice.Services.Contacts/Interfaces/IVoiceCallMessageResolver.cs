using MIN.Core.Stores.Contracts.Models;

namespace MIN.Voice.Services.Contacts.Interfaces;

/// <summary>
/// Помошник в поиске сообщения, представляющее звонок
/// </summary>
public interface IVoiceCallMessageResolver
{
    /// <summary>
    /// Получить id сообщения, представляющее звонок по id подкомнаты
    /// </summary>
    Guid? GetVoiceCallMessageIdOutOfSubRoomId(RoomContext context, int subRoomId);
}
