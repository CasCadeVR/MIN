using MIN.Sessions.Core.Transport.Contracts.Enums;

namespace MIN.Sessions.Core.Transport.Contracts.Events;

/// <summary>
/// Аргументы событие получения данных от приложения
/// </summary>
public sealed class ProcessTransportMessageEventArgs : EventArgs
{
    /// <summary>
    /// Идентификатор комнаты
    /// </summary>
    public Guid RoomId { get; init; }

    /// <summary>
    /// Идентификатор подкомнаты
    /// </summary>
    public int SubRoomId { get; init; }

    /// <summary>
    /// Роль 
    /// </summary>
    public SessionProcessRole Role { get; init; }

    /// <summary>
    /// Полученные данные
    /// </summary>
    public byte[] Data { get; init; } = null!;

    /// <summary>
    /// Отправитель
    /// </summary>
    public Guid SenderId { get; init; }
}
