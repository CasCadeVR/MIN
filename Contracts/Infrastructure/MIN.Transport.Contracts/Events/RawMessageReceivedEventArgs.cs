using MIN.Messaging.Contracts.Entities;

namespace MIN.Transport.Contracts.Events;

/// <summary>
/// Аргументы события получения сырых данных от транспорта
/// </summary>
public sealed class RawMessageReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Полученные данные (байты)
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; }

    /// <summary>
    /// Отправитель сообщения
    /// </summary>
    public ParticipantInfo Sender { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RawMessageReceivedEventArgs"/>
    /// </summary>
    public RawMessageReceivedEventArgs(byte[] data, Guid roomId, ParticipantInfo sender)
    {
        Data = data;
        RoomId = roomId;
        Sender = sender;
    }
}
