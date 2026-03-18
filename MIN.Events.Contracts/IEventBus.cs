namespace MIN.Events.Contracts;

/// <summary>
/// Шина событий для внутрипроцессной коммуникации
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Публикует событие в шину
    /// </summary>
    Task PublishAsync<T>(T eventMessage, CancellationToken cancellationToken = default) where T : IEvent;

    /// <summary>
    /// Подписывается на события указанного типа
    /// </summary>
    IDisposable Subscribe<T>(Func<T, CancellationToken, Task> handler) where T : IEvent;

    /// <summary>
    /// Подписывается на события указанного типа с фильтром
    /// </summary>
    IDisposable Subscribe<T>(Func<T, bool> filter, Func<T, CancellationToken, Task> handler) where T : IEvent;
}
