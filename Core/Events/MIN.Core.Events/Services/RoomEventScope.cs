using MIN.Core.Events.Contracts.Interfaces;

namespace MIN.Core.Events.Services;

/// <inheritdoc cref="IEventBus"/>
public sealed class RoomEventScope : IEventScope
{
    private readonly IEventBus eventBus;
    private readonly Guid roomId;
    private readonly HashSet<IDisposable> tokens = [];

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomEventScope"/>
    /// </summary>
    public RoomEventScope(IEventBus eventBus, Guid roomId)
    {
        this.eventBus = eventBus;
        this.roomId = roomId;
    }

    IDisposable IEventScope.Subscribe<T>(Func<T, CancellationToken, Task> handler)
    {
        Task Wrapped(T e, CancellationToken ct) => e.RoomId == roomId ? handler(e, ct) : Task.CompletedTask;
        var token = eventBus.Subscribe<T>(Wrapped);
        tokens.Add(token);
        return token;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        foreach (var token in tokens)
        {
            token.Dispose();
        }

        tokens.Clear();
    }
}
