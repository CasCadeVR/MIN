namespace MIN.Core.Services.Contracts.Interfaces.Rooms;

/// <summary>
/// Сервис по отключению клиента с причиной
/// </summary>
public interface IGracefulDisconnector
{
    /// <summary>
    /// Отключить клиента, указав причину
    /// </summary>
    Task DisconnectWithReasonAsync(Guid connectionId, Guid roomId, string reason, int timeoutMs = 5000);
}
