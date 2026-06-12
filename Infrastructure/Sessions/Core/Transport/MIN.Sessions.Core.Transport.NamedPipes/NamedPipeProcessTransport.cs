using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text.Json;
using MIN.Sessions.Core.Services.Contracts.Models;
using MIN.Sessions.Core.Transport.Contracts.Events;
using MIN.Sessions.Core.Transport.Contracts.Interfaces;
using MIN.Sessions.Core.Transport.Contracts.Models;

namespace MIN.Sessions.Core.Transport.NamedPipes;

/// <summary>
/// <inheritdoc cref="ISessionProcessTransport"/>
/// </summary>
public sealed class NamedPipeProcessTransport : ISessionProcessTransport
{
    private readonly ConcurrentDictionary<ProcessContext, NamedPipeServerStream> connections = [];
    private readonly CancellationTokenSource cts = new();
    private readonly SemaphoreSlim writeLock = new(1, 1);
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

    async Task ISessionProcessTransport.WaitForConnectionAsync(ProcessContext context, int timeOutMs, CancellationToken cancellationToken)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new InvalidOperationException("Windows only");
        }

        var server = new NamedPipeServerStream(
            pipeName,
            PipeDirection.InOut,
            NamedPipeServerStream.MaxAllowedServerInstances,
            PipeTransmissionMode.Message,
            options: PipeOptions.Asynchronous);

        var connectionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        connectionCts.CancelAfter(timeOutMs);
        await server.WaitForConnectionAsync(connectionCts.Token);

        connections.TryAdd(context, server);
        _ = ReadLoopAsync(context, server, cts.Token);
    }

    bool ISessionProcessTransport.IsConnectionExists(ProcessContext context)
        => connections.ContainsKey(context);

    private async Task ReadLoopAsync(ProcessContext context, NamedPipeServerStream stream, CancellationToken cancellationToken)
    {
        try
        {
            var lengthBuf = new byte[4];
            while (!cancellationToken.IsCancellationRequested)
            {
                await stream.ReadExactlyAsync(lengthBuf, cancellationToken);
                var length = BitConverter.ToInt32(lengthBuf);
                var body = new byte[length];
                await stream.ReadExactlyAsync(body, cancellationToken);

                MessageReceived?.Invoke(this, new ProcessTransportMessageEventArgs
                {
                    Context = context,
                    Data = body,
                });
            }
        }
        catch (EndOfStreamException) { }
        catch (OperationCanceledException) { }
        finally
        {
            connections.TryRemove(context, out _);
        }
    }

    async Task ISessionProcessTransport.SendAsync(byte[] data, ProcessContext context, CancellationToken cancellationToken)
    {
        if (connections.TryGetValue(context, out var stream))
        {
            await writeLock.WaitAsync(cancellationToken);
            try
            {
                var lengthBuf = BitConverter.GetBytes(data.Length);
                await stream.WriteAsync(lengthBuf, cancellationToken);
                await stream.WriteAsync(data, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
            finally
            {
                writeLock.Release();
            }
        }
    }

    Task ISessionProcessTransport.DisconnectAsync(ProcessContext context)
    {
        if (connections.TryRemove(context, out var stream))
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
