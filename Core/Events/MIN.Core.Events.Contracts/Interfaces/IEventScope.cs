using MIN.Core.Events.Contracts.Models;

namespace MIN.Core.Events.Contracts.Interfaces;

/// <summary>
/// Scope события, куда нужно выложить событие
/// </summary>
public interface IEventScope : IDisposable
{
    /// <summary>
    /// Подписаться на событие внутри одного Scope
    /// </summary>
    IDisposable Subscribe<T>(Func<T, CancellationToken, Task> handler) where T : BaseEvent, IRoomScopedEvent;
}
