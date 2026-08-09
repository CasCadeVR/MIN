namespace MIN.Chat.Services.Contracts.Interfaces;

/// <summary>
/// Сервис для работы с звонками
/// </summary>
public interface IChatVoiceService
{
    /// <summary>
    /// Начать звонок в комнате
    /// </summary>
    Task StartCallAsync(Guid roomId, CancellationToken ct = default);

    /// <summary>
    /// Присоединиться к существующему звонку
    /// </summary>
    Task JoinCallAsync(Guid roomId, int subRoomId, CancellationToken ct = default);

    /// <summary>
    /// Покинуть звонок
    /// </summary>
    Task LeaveCallAsync(Guid roomId, int subRoomId, CancellationToken ct = default);
}
