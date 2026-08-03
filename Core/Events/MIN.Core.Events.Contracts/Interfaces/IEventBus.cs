using MIN.Core.Events.Contracts.Models;

namespace MIN.Core.Events.Contracts.Interfaces;

/// <summary>
/// Шина событий для внутрипроцессной коммуникации
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Публикует событие в шину
    /// </summary>
    Task PublishAsync<T>(T eventMessage, CancellationToken cancellationToken = default) where T : BaseEvent;

    /// <summary>
    /// Подписывается на события указанного типа
    /// </summary>
    IDisposable Subscribe<T>(Func<T, CancellationToken, Task> handler) where T : BaseEvent;

    /// <summary>
    /// Создать Scope и сгруппировать под события по идентификатору
    /// </summary>
    IEventScope CreateScope(Guid roomId);
}
