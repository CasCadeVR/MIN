using System.Collections.Concurrent;
using MIN.Services.Contracts.Interfaces;
using MIN.Services.Contracts.Interfaces.Messaging;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Entities;
using MIN.Events.Contracts;
using MIN.Events.Events;
using MIN.Entities.Contracts.Models;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Transport.Contracts.Events;
using MIN.Transport.Contracts.Interfaces;

namespace MIN.Services
{
    /// <inheritdoc cref="IRoomService"/>
    public sealed class RoomService : IRoomService, IAsyncDisposable
    {
        private readonly ITransport transport;
        private readonly IEventBus eventBus;
        private readonly IMessageService messageService;
        private readonly IMessageEncryptor encryptor;
        private readonly ILoggerProvider logger;

        private readonly ConcurrentDictionary<Guid, Room> roomById = new();
        private readonly ConcurrentDictionary<Guid, Guid> roomIdByConnectionId = new();

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
        {
            if (roomById.TryGetValue(roomId, out var room))
            {
                return room;
            }

            throw new InvalidOperationException($"Room with id {roomId} not found");
        }

        void IRoomService.SetRoom(Guid connectionId, Room room)
        {
            roomById[room.Id] = room;
            roomIdByConnectionId[connectionId] = room.Id;
        }

        void IRoomService.AddParticipant(Guid roomId, ParticipantInfo participant)
        {
            if (!roomById.TryGetValue(roomId, out var room))
            {
                throw new InvalidOperationException($"Room with id {roomId} not found");
            }

            room.CurrentParticipants.Add(participant);
        }

        void IRoomService.RemoveParticipant(Guid roomId, Guid participantId)
        {
            if (!roomById.TryGetValue(roomId, out var room))
            {
                throw new InvalidOperationException($"Room with id {roomId} not found");
            }

            var existing = room.CurrentParticipants.FirstOrDefault(p => p.Id == participantId)
                ?? throw new InvalidOperationException($"Participant with id {participantId} not found in room {roomId}");

            room.CurrentParticipants.Remove(existing);
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
            logger.Log($"Connected to room {roomId} with connection {connectionId}");

            var handshakeMessage = await encryptor.CreateSelfHandshakeMessageAsync();
            await messageService.SendAsync(handshakeMessage, roomId, connectionId, cancellationToken);

            return connectionId;
        }

        async Task IRoomService.DisconnectAsync(Guid roomId, Guid connectionId)
        {
            await transport.DisconnectAsync(roomId, connectionId);
            roomIdByConnectionId.TryRemove(connectionId, out _);
        }

        /// <summary>
        /// Запустить сервис (подписывается на события транспорта)
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            transport.ConnectionStateChanged += OnConnectionStateChanged;
            await Task.CompletedTask;
        }

        private async void OnConnectionStateChanged(object? sender, ConnectionStateChangedEventArgs e)
        {
            try
            {
                await eventBus.PublishAsync(new ConnectionStatusChangedEvent()
                {
                    RoomId = e.RoomId,
                    ConnectionId = e.ConnectionId,
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
            transport.ConnectionStateChanged -= OnConnectionStateChanged;
            cts?.Dispose();
            await Task.CompletedTask;
        }
    }
}
