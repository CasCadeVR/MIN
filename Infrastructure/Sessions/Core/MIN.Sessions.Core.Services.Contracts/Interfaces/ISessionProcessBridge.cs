using MIN.Sessions.Core.Messaging.Contracts.Models;
using MIN.Sessions.Core.Services.Contracts.Models;

namespace MIN.Sessions.Core.Services.Contracts.Interfaces;

/// <summary>
/// Мост для общение с приложениями
/// </summary>
public interface ISessionProcessBridge
{
    /// <summary>
    /// Запустить сервис, слушая все сообщения с приложениями
    /// </summary>
    Task StartListeningAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Дождаться получения сообщения о готовности
    /// </summary>
    Task<bool> WaitForReadyMessage(ProcessContext context, int timeOutMs, CancellationToken cancellationToken);

    /// <summary>
    /// Отправить междупроцессорное сообщение
    /// </summary>
    Task SendIpcMessage(IpcMessage message, ProcessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить междупроцессорные данные
    /// </summary>
    Task SendData(byte[] data, ProcessContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить список соединений с приложением  
    /// </summary>
    IEnumerable<ProcessContext> GetConnections(Guid roomId, int subRoomId);

    /// <summary>
    /// Остановить сервис
    /// </summary>
    Task StopListeningAsync(CancellationToken cancellationToken = default);
}
