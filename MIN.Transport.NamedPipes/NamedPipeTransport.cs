using System.Collections.Concurrent;
using MIN.Cryptography.Contracts.Interfaces;
using MIN.Messaging.Contracts.Entities;
using MIN.Serialization.Contracts;
using MIN.Services.Contracts.Interfaces;
using MIN.Transport.Contracts.Interfaces;
using MIN.Transport.Contracts.Constants;
using MIN.Transport.Contracts.Events;
using MIN.Transport.Contracts.Models;
using MIN.Transport.NamedPipes.Client;
using MIN.Transport.NamedPipes.Models;
using MIN.Transport.NamedPipes.Server;

namespace MIN.Transport.NamedPipes;

/// <summary>
/// Реализация транспорта на основе Named Pipes
/// </summary>
public sealed class NamedPipeTransport : ITransport
{
    private readonly IMessageSerializer serializer;
    private readonly IMessageEncryptor encryptor;
    private readonly ILoggerProvider logger;
    private readonly ConcurrentDictionary<Guid, NamedPipeServer> servers = new();
    private readonly ConcurrentDictionary<Guid, NamedPipeClient> clients = new();

    public NamedPipeTransport(
        IMessageSerializer serializer,
        IMessageEncryptor encryptor,
        ILoggerProvider logger)
    {
        this.serializer = serializer;
        this.encryptor = encryptor;
        this.logger = logger;
    }

    /// <inheritdoc />
    public event EventHandler<RawMessageReceivedEventArgs>? RawMessageReceived;

    /// <inheritdoc />
    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    async Task ITransport.StartLHostingAsync(Guid roomId, IEndpoint endpoint, CancellationToken cancellationToken)
    {
        if (endpoint is not NamedPipeEndpoint namedPipeEndpoint)
        {
            throw new ArgumentException("Endpoint must be NamedPipeEndpoint", nameof(endpoint));
        }

        if (servers.ContainsKey(roomId))
        {
            return;
        }

        // TODO: максимальное количество участников нужно получить из конфигурации
        const int maxParticipants = TransportConstants.TheoraticallyPossibleMaximumRoomSize;

        var server = new NamedPipeServer(namedPipeEndpoint, maxParticipants, serializer, encryptor, logger);
        server.RawMessageReceived += OnServerMessageReceived;
        server.ParticipantConnected += OnParticipantConnected;
        server.ParticipantDisconnected += OnParticipantDisconnected;

        servers[roomId] = server;
        await server.StartAsync(cancellationToken);
    }

    async Task ITransport.StopHostingAsync(Guid roomId)
    {
        if (servers.TryRemove(roomId, out var server))
        {
            await server.DisposeAsync();
        }
    }

    async Task ITransport.ConnectAsync(Guid roomId, IEndpoint endpoint, ParticipantInfo localParticipant, int timeoutMs, CancellationToken cancellationToken)
    {
        if (endpoint is not NamedPipeEndpoint namedPipeEndpoint)
        {
            throw new ArgumentException("Endpoint must be NamedPipeEndpoint", nameof(endpoint));
        }

        if (clients.ContainsKey(roomId))
        {
            return;
        }

        var client = new NamedPipeClient(namedPipeEndpoint, localParticipant, serializer, encryptor, logger);
        client.RawMessageReceived += OnClientMessageReceived;
        client.Disconnected += OnClientDisconnected;

        clients[roomId] = client;
        await client.ConnectAsync(timeoutMs, cancellationToken);

        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(roomId, localParticipant.Id, true));
    }

    async Task ITransport.DisconnectAsync(Guid roomId)
    {
        if (clients.TryRemove(roomId, out var client))
        {
            await client.DisposeAsync();
        }
    }

    async Task ITransport.SendAsync(byte[] data, Guid roomId, ParticipantInfo target, CancellationToken cancellationToken)
    {
        // Отправляем конкретному участнику – если есть сервер для комнаты, отправляем через него
        // Пока предположим, что target – это сервер (для клиента), а сервер может отправлять конкретному клиенту.
        // Для клиента: если мы клиент, то отправляем только хосту.
        // Для сервера: нужно найти соединение с target и отправить.

        // Упрощённая логика: если есть клиент для комнаты, отправляем через него (предполагаем, что target – сервер)
        var clientRoomId = clients.FirstOrDefault(c => c.Value.IsConnected).Key;
        if (clientRoomId != Guid.Empty)
        {
            var client = clients[clientRoomId];
            await client.SendAsync(data, cancellationToken);
            return;
        }

        // Если есть сервер для комнаты, ищем участника
        var serverRoomId = servers.FirstOrDefault(s => s.Value != null).Key;
        if (serverRoomId != Guid.Empty)
        {
            var server = servers[serverRoomId];
            await server.SendToParticipantAsync(data, target, cancellationToken);
            return;
        }

        throw new InvalidOperationException("No active connection to send");
    }

    async Task ITransport.BroadcastAsync(byte[] data, Guid roomId, List<ParticipantInfo>? toExclude, CancellationToken cancellationToken)
    {
        if (servers.TryGetValue(roomId, out var server))
        {
            await server.BroadcastAsync(data, toExclude, cancellationToken);
        }
        else if (clients.TryGetValue(roomId, out var client))
        {
            await client.SendAsync(data, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException($"No active connection for room {roomId}");
        }
    }

    private void OnServerMessageReceived(object? sender, (byte[] Data, ParticipantInfo Sender) args)
    {
        var roomId = servers.FirstOrDefault(x => x.Value == sender).Key;
        if (roomId == Guid.Empty)
        {
            return;
        }

        RawMessageReceived?.Invoke(this, new RawMessageReceivedEventArgs(args.Data, roomId, args.Sender));
    }

    private void OnClientMessageReceived(object? sender, byte[] data)
    {
        var foundElement = clients.FirstOrDefault(x => x.Value == sender);
        var roomId = foundElement.Key;
        if (roomId == Guid.Empty)
        {
            return;
        }

        var client = foundElement.Value;
        var senderParticipant = client.ServerParticipant ?? throw new InvalidOperationException("No server participant");
        RawMessageReceived?.Invoke(this, new RawMessageReceivedEventArgs(data, roomId, senderParticipant));
    }

    private void OnParticipantConnected(object? sender, ParticipantInfo participant)
    {
        var roomId = servers.FirstOrDefault(x => x.Value == sender).Key;
        if (roomId == Guid.Empty)
        {
            return;
        }

        ConnectionStateChanged?.Invoke(this,
            new ConnectionStateChangedEventArgs(roomId, participant.Id, true));
    }

    private void OnParticipantDisconnected(object? sender, (string? reason, ParticipantInfo participant) disconnectInfo)
    {
        var roomId = servers.FirstOrDefault(x => x.Value == sender).Key;
        if (roomId == Guid.Empty)
        {
            return;
        }

        ConnectionStateChanged?.Invoke(this,
            new ConnectionStateChangedEventArgs(roomId, disconnectInfo.participant.Id, false, disconnectInfo.reason));
    }

    private void OnClientDisconnected(object? sender, string? reason)
    {
        var foundElement = clients.FirstOrDefault(x => x.Value == sender);
        var roomId = foundElement.Key;
        if (roomId == Guid.Empty)
        {
            return;
        }

        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(roomId, foundElement.Value.ClientParticipant.Id, false, reason));
        clients.TryRemove(roomId, out _);
    }
}
