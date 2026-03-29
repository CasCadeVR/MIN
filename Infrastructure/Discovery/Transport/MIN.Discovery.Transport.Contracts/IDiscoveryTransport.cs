using MIN.Discovery.Transport.Contracts.Events;

namespace MIN.Discovery.Transport.Contracts;

/// <summary>
/// Транспортный уровень для широковещательного обнаружения комнат
/// </summary>
public interface IDiscoveryTransport
{
    /// <summary>
    /// Событие получения сырых данных от транспорта
    /// </summary>
    event EventHandler<DiscoveryRawMessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    /// Отправить данные
    /// </summary>
    Task SendAsync(byte[] data, string? destination, TimeSpan? timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Начать прослушивание широковещательных сообщений, с отправкой ответа
    /// </summary>
    Task StartListeningAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить ответные данные
    /// </summary>
    Task ResponseWithData(byte[] responseData, TimeSpan? timeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить прослушивание
    /// </summary>
    Task StopListeningAsync();
}
