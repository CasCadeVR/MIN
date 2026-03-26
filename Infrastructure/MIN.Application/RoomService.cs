using System.Collections.Concurrent;
using MIN.Application.Contracts.Interfaces;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Entities;
using MIN.Events.Contracts;
using MIN.Events.Events;
using MIN.Messaging.Contracts.Entities;
using MIN.Services.Contracts.Interfaces;
using MIN.Transport.Contracts.Events;
using MIN.Transport.Contracts.Interfaces;

namespace MIN.Application
{
    /// <inheritdoc cref="IRoomService"/>
    public sealed class RoomService : IRoomService, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IEventBus eventBus;
        private readonly IMessageService messageService;
        private readonly IMessageEncryptor encryptor;
        private readonly ILoggerProvider logger;
        private readonly ConcurrentDictionary<Guid, Room> rooms = new();
        private CancellationTokenSource? cts;
        private bool disposed;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RoomService"/>
        /// </summary>
        public RoomService(ITransport transport,
            IEventBus eventBus,
            IMessageService messageService,
            IMessageEncryptor encryptor,
            ILoggerProvider logger)
        {
            this.transport = transport;
            this.eventBus = eventBus;
            this.messageService = messageService;
            this.encryptor = encryptor;
            this.logger = logger;
        }

        Room IRoomService.GetRoom(Guid roomId)
            => rooms.Values.Where(x => x.Id == roomId).FirstOrDefault()
                ?? throw new ArgumentNullException($"Не удалось найти комнату с id {roomId} среди подключённых");

        void IRoomService.SetRoom(Guid connectionId, Room room)
        {
            rooms[connectionId] = room;
        }

        void IRoomService.AddParticipant(Guid roomId, ParticipantInfo participant)
        {
            var existing = rooms.Values.Where(x => x.Id == roomId).FirstOrDefault()
                ?? throw new ArgumentNullException($"Не удалось найти комнату с id {roomId} среди подключённых");

            existing.CurrentParticipants.Add(participant);
        }

        void IRoomService.RemoveParticipant(Guid roomId, Guid participantId)
        {
            var existing = rooms.Values.Where(x => x.Id == roomId).FirstOrDefault()
                ?? throw new ArgumentNullException($"Не удалось найти комнату с id {roomId} среди подключённых");

            var existingParticipant = existing.CurrentParticipants.Where(x => x.Id == participantId).FirstOrDefault()
                ?? throw new ArgumentNullException($"Не удалось найти участника с id {participantId} чтобы удалить его");

            existing.CurrentParticipants.Remove(existingParticipant);
        }

        async Task IRoomService.StartHostingAsync(Guid roomId, IEndpoint endpoint, CancellationToken cancellationToken)
        {
            await transport.StartHostingAsync(roomId, endpoint, cancellationToken);
        }

        async Task IRoomService.StopHostingAsync(Guid roomId)
        {
            await transport.StopHostingAsync(roomId);
        }

        async Task<Guid> IRoomService.ConnectAsync(Guid roomId, IEndpoint endpoint, int timeoutMs, CancellationToken cancellationToken)
        {
            var connectionId = await transport.ConnectAsync(roomId, endpoint, timeoutMs, cancellationToken);

            logger.Log($"Успешно подключился к комнате с id {roomId}");

            var handShakeMessage = await encryptor.CreateSelfHandshakeMessageAsync();

            await messageService.SendAsync(handShakeMessage, roomId, connectionId, cancellationToken);

            return connectionId;
        }

        async Task IRoomService.DisconnectAsync(Guid roomId, Guid connectionId)
        {
            await transport.DisconnectAsync(roomId, connectionId);
        }

        /// <summary>
        /// Запустить сервис
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            transport.ConnectionStateChanged += async (sender, e)
                => await OnConnectionStateChanged(sender, e);
        }

        private async Task OnConnectionStateChanged(object? sender, ConnectionStateChangedEventArgs e)
        {
            try
            {
                await eventBus.PublishAsync(new ConnectionStatusChangedEvent()
                {
                    RoomId = e.RoomId,
                    ErrorMessage = e.Reason,
                    IsConnected = e.IsConnected,
                }, cts!.Token);
            }
            catch (Exception ex)
            {
                logger.Log($"Произошла ошибка во время обработки изменения состояния подключения: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            cts?.Cancel();
            transport.ConnectionStateChanged -= async (sender, e)
                => await OnConnectionStateChanged(sender, e);
            cts?.Dispose();
            await Task.CompletedTask;
        }
    }
}
