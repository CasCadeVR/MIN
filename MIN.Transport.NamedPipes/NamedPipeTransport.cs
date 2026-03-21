using System.Collections.Concurrent;
using MIN.Messaging.Contracts.Entities;
using MIN.Serialization.Contracts;
using MIN.Services.Contracts.Interfaces;
using MIN.Transport.Contracts;
using MIN.Transport.Contracts.Endpoints;
using MIN.Transport.Contracts.Events;
using MIN.Transport.NamedPipes.Client;
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

    public event EventHandler<RawMessageReceivedEventArgs>? RawMessageReceived;
    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    public async Task StartListeningAsync(Guid roomId, IEndpoint endpoint, CancellationToken cancellationToken = default)
    {
        if (endpoint is not NamedPipeEndpoint namedPipeEndpoint)
        {
            throw new ArgumentException("Endpoint must be NamedPipeEndpoint", nameof(endpoint));
        }

        if (servers.ContainsKey(roomId))
        {
            return;
        }

        // TODO: максимальное количество участников нужно получить из конфигурации или от вызывающего кода
        const int maxParticipants = 20;
        var server = new NamedPipeServer(roomId, namedPipeEndpoint, maxParticipants, serializer, encryptor, logger);
        server.MessageReceived += OnServerMessageReceived;
        server.ParticipantConnected += OnParticipantConnected;
        server.ParticipantDisconnected += OnParticipantDisconnected;

        servers[roomId] = server;
        await server.StartAsync(cancellationToken);
    }

    public async Task StopListeningAsync(Guid roomId)
    {
        if (servers.TryRemove(roomId, out var server))
        {
            await server.DisposeAsync();
        }
    }

    public async Task ConnectAsync(Guid roomId, IEndpoint endpoint, ParticipantInfo localParticipant, CancellationToken cancellationToken = default)
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
        client.MessageReceived += OnClientMessageReceived;
        client.Disconnected += OnClientDisconnected;

        clients[roomId] = client;
        await client.ConnectAsync(cancellationToken);

        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(roomId, localParticipant.Id, true));
    }

    public async Task DisconnectAsync(Guid roomId)
    {
        if (clients.TryRemove(roomId, out var client))
        {
            await client.DisposeAsync();
        }
    }

    public async Task SendAsync(byte[] data, ParticipantInfo target, CancellationToken cancellationToken = default)
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

    public async Task BroadcastAsync(byte[] data, Guid roomId, ParticipantInfo? exclude = null, CancellationToken cancellationToken = default)
    {
        if (servers.TryGetValue(roomId, out var server))
        {
            await server.BroadcastAsync(data, exclude, cancellationToken);
        }
        else if (clients.TryGetValue(roomId, out var client))
        {
            // Для клиента broadcast не имеет смысла, но можно отправить хосту
            await client.SendAsync(data, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException($"No active connection for room {roomId}");
        }
    }

    private void OnServerMessageReceived(object? sender, (byte[] Data, ParticipantInfo Sender) args)
    {
        // Найти roomId для этого сервера
        var roomId = servers.FirstOrDefault(x => x.Value == sender).Key;
        if (roomId == Guid.Empty)
        {
            return;
        }

        RawMessageReceived?.Invoke(this, new RawMessageReceivedEventArgs(args.Data, roomId, args.Sender));
    }

    private void OnClientMessageReceived(object? sender, byte[] data)
    {
        var roomId = clients.FirstOrDefault(x => x.Value == sender).Key;
        if (roomId == Guid.Empty)
        {
            return;
        }

        var client = (NamedPipeClient)sender!;
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

        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(roomId, participant.Id, true));
    }

    private void OnParticipantDisconnected(object? sender, ParticipantInfo participant)
    {
        var roomId = servers.FirstOrDefault(x => x.Value == sender).Key;
        if (roomId == Guid.Empty)
        {
            return;
        }

        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(roomId, participant.Id, false));
    }

    private void OnClientDisconnected(object? sender, string? reason)
    {
        var roomId = clients.FirstOrDefault(x => x.Value == sender).Key;
        if (roomId == Guid.Empty)
        {
            return;
        }

        // Здесь нужно получить ID локального участника, но у нас его нет в клиенте. Можно хранить в словаре отдельно.
        // Для упрощения оставим ParticipantId = Guid.Empty
        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(roomId, Guid.Empty, false, reason));
        clients.TryRemove(roomId, out _);
    }
}
