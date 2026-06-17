namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Сервис по обработке исключений на уровне сети
/// </summary>
public interface INetworkErrorHandler
{
    /// <summary>
    /// Получить причину отключения
    /// </summary>
    /// <remarks>
    /// null, если учатсник вышел добровольно
    /// </remarks>
    string? GetDisconnectDetailsFor(Guid participantId, Guid roomId);

    /// <summary>
    /// Отключить клиента, указав причину
    /// </summary>
    /// <remarks>
    /// если critical = true - нужно разорвать соединение
    /// </remarks>
    Task SendErrorAsync(string message, Guid recipientId, Guid roomId, bool critical = false, int timeoutMs = 5000);
}
