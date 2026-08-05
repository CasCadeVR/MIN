namespace MIN.Core.Services.Contracts.Interfaces.Moderation;

/// <summary>
/// Сервис по обработке исключений на уровне сети
/// </summary>
public interface INetworkErrorHandler
{
    /// <summary>
    /// Отключить клиента, указав причину
    /// </summary>
    /// <remarks>
    /// если critical = true - нужно разорвать соединение
    /// </remarks>
    Task SendErrorAsync(string message, Guid recipientId, Guid roomId, bool critical = false, int timeoutMs = 5000);
}
