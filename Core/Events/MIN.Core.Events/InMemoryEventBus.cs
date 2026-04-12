using System.Collections.Concurrent;
using MIN.Core.Events.Contracts;
using MIN.Core.Events.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Core.Events
{
    /// <inheritdoc cref="IEventBus"/>
    public sealed class InMemoryEventBus : IEventBus, IAsyncDisposable
    {
        private readonly ConcurrentDictionary<Type, List<Func<object, CancellationToken, Task>>> handlers = new();
        private readonly ILoggerProvider logger;
        private readonly CancellationTokenSource cts = new();

        private bool disposed;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="InMemoryEventBus"/>
        /// </summary>
        public InMemoryEventBus(ILoggerProvider logger)
        {
            this.logger = logger;
        }

        async Task IEventBus.PublishAsync<T>(T eventMessage, CancellationToken cancellationToken)
        {
            var eventType = typeof(T);
            if (!this.handlers.TryGetValue(eventType, out var handlers))
            {
                return;
            }

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
            var tasks = handlers.Select(handler => SafeExecuteHandler(handler, eventMessage, linkedCts.Token));

            await Task.WhenAll(tasks);
        }

        IDisposable IEventBus.Subscribe<T>(Func<T, CancellationToken, Task> handler)
        {
            var eventType = typeof(T);
            var handlers = this.handlers.GetOrAdd(eventType, _ => []);

            Task wrappedHandler(object eventObj, CancellationToken ct) => handler((T)eventObj, ct);

            lock (handlers)
            {
                handlers.Add(wrappedHandler);
            }

            return new SubscriptionToken(() => Unsubscribe(eventType, wrappedHandler));
        }

        IDisposable IEventBus.Subscribe<T>(Func<T, bool> filter, Func<T, CancellationToken, Task> handler)
        {
            var eventType = typeof(T);
            var handlers = this.handlers.GetOrAdd(eventType, _ => new List<Func<object, CancellationToken, Task>>());

            Func<object, CancellationToken, Task> wrappedHandler = async (eventObj, ct) =>
            {
                var typedEvent = (T)eventObj;
                if (filter(typedEvent))
                {
                    await handler(typedEvent, ct);
                }
            };

            lock (handlers)
            {
                handlers.Add(wrappedHandler);
            }

            return new SubscriptionToken(() => Unsubscribe(eventType, wrappedHandler));
        }

        private async Task SafeExecuteHandler(Func<object, CancellationToken, Task> handler, IEvent eventMessage, CancellationToken cancellationToken)
        {
            try
            {
                await handler(eventMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Log($"Произошла ошибка во время обработки события: {eventMessage.GetType().Name}: {ex.Message}", LogLevel.Error);
            }
        }

        private void Unsubscribe(Type eventType, Func<object, CancellationToken, Task> handler)
        {
            if (!this.handlers.TryGetValue(eventType, out var handlers))
            {
                return;
            }

            lock (handlers)
            {
                handlers.Remove(handler);
                if (handlers.Count == 0)
                {
                    this.handlers.TryRemove(eventType, out _);
                }
            }
        }

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
        public async ValueTask DisposeAsync()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            await cts.CancelAsync();
            cts.Dispose();
            handlers.Clear();
        }
    }
}
