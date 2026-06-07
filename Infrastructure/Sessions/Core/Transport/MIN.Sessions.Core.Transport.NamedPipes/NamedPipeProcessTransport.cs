using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text.Json;
using MIN.Sessions.Core.Transport.Contracts.Enums;
using MIN.Sessions.Core.Transport.Contracts.Events;
using MIN.Sessions.Core.Transport.Contracts.Interfaces;
using MIN.Sessions.Core.Transport.Contracts.Models;

namespace MIN.Sessions.Core.Transport.NamedPipes;

/// <summary>
/// <inheritdoc cref="ISessionProcessTransport"/>
/// </summary>
public sealed class NamedPipeProcessTransport : ISessionProcessTransport
{
    private readonly ConcurrentDictionary<(Guid, int, SessionProcessRole), NamedPipeServerStream> connections = [];
    private readonly CancellationTokenSource cts = new();
    private string pipeName = string.Empty;

    /// <inheritdoc/>
    public event EventHandler<ProcessTransportMessageEventArgs>? MessageReceived;

    Task ISessionProcessTransport.StartAsync(Guid roomId, CancellationToken cancellationToken)
    {
        pipeName = $"MIN_{roomId}";
        return Task.CompletedTask;
    }

    string ISessionProcessTransport.GetConnectionString() =>
        JsonSerializer.Serialize(new ConnectionInfo
        {
            Type = "pipe",
            Value = pipeName,
        });

    async Task<TransportConnection> ISessionProcessTransport.WaitForConnectionAsync(
        Guid roomId, int subRoomId, SessionProcessRole role, CancellationToken ct)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new InvalidOperationException("Windowns only");
        }

        var server = new NamedPipeServerStream(
            pipeName,
            PipeDirection.InOut,
            NamedPipeServerStream.MaxAllowedServerInstances,
            PipeTransmissionMode.Message);

        await server.WaitForConnectionAsync(ct);

        var key = (roomId, subRoomId, role);
        connections.TryAdd(key, server);
        _ = ReadLoopAsync(key, server, cts.Token);

        return new TransportConnection(roomId, role, subRoomId, server, server);
    }

    private async Task ReadLoopAsync(
        (Guid, int, SessionProcessRole) key, NamedPipeServerStream stream, CancellationToken ct)
    {
        try
        {
            var lengthBuf = new byte[4];
            while (!ct.IsCancellationRequested)
            {
                await stream.ReadExactlyAsync(lengthBuf, ct);
                var length = BitConverter.ToInt32(lengthBuf);
                var body = new byte[length];
                await stream.ReadExactlyAsync(body, ct);

                MessageReceived?.Invoke(this, new ProcessTransportMessageEventArgs
                {
                    RoomId = key.Item1,
                    SubRoomId = key.Item2,
                    Role = key.Item3,
                    Data = body,
                });
            }
        }
        catch (Exception)
        {
            connections.TryRemove(key, out _);
        }
    }

    async Task ISessionProcessTransport.SendAsync(Guid roomId, int subRoomId, SessionProcessRole role, byte[] data, CancellationToken ct)
    {
        if (connections.TryGetValue((roomId, subRoomId, role), out var stream))
        {
            var lengthBuf = BitConverter.GetBytes(data.Length);
            await stream.WriteAsync(lengthBuf, ct);
            await stream.WriteAsync(data, ct);
            await stream.FlushAsync(ct);
        }
    }

    Task ISessionProcessTransport.DisconnectAsync(Guid roomId, int subRoomId, SessionProcessRole role)
    {
        if (connections.TryRemove((roomId, subRoomId, role), out var stream))
        {
            stream.Dispose();
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync()
    {
        await cts.CancelAsync();

        foreach (var server in connections.Values)
        {
            server.Dispose();
        }
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    async ValueTask IAsyncDisposable.DisposeAsync() => await StopAsync();
}
