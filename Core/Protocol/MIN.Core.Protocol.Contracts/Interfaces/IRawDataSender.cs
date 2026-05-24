namespace MIN.Core.Protocol.Contracts.Interfaces;

/// <summary>
/// Отправитель сырых данных
/// </summary>
public interface IRawDataSender
{
    /// <summary>
    /// Отправить сырые данные по идентификатору комнаты и соединения
    /// </summary>
    Task SendAsync(byte[] data, Guid roomId, Guid connectionId, CancellationToken cancellationToken);
}
