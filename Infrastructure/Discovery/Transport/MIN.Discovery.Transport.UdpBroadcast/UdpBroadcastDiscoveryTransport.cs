using System.Net;
using System.Net.Sockets;
using MIN.Discovery.Transport.Contracts;
using MIN.Discovery.Transport.Contracts.Events;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;

namespace MIN.Discovery.Transport.UdpBroadcast;

/// <inheritdoc cref="IDiscoveryTransport"/>
public sealed class UdpBroadcastDiscoveryTransport : IDiscoveryTransport, IAsyncDisposable
{
    private readonly ILoggerProvider logger;
    private int port;
    private UdpClient? listener;
    private CancellationTokenSource? listenerCts;
    private bool isListening;

    /// <inheritdoc />
    public event EventHandler<DiscoveryRawMessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="UdpBroadcastDiscoveryTransport"/>
    /// </summary>
    public UdpBroadcastDiscoveryTransport(ISettingsProvider settingsProvider, ILoggerProvider logger)
    {
        this.logger = logger;

        port = settingsProvider.GetSettings().DiscoveryPort;
        settingsProvider.OnSettingsSaved += () => port = settingsProvider.GetSettings().DiscoveryPort;
    }

    /// <inheritdoc />
    public Task StartListeningAsync(CancellationToken cancellationToken = default)
    {
        if (isListening)
        {
            return Task.CompletedTask;
        }

        listenerCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        listener = new UdpClient(new IPEndPoint(IPAddress.Any, port));
        isListening = true;
        _ = ListenLoopAsync(listenerCts.Token);
        return Task.CompletedTask;
    }

    private async Task ListenLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var result = await listener!.ReceiveAsync(ct);
                if (!UdpPacketHelper.TryUnpack(result.Buffer, out var payload))
                {
                    continue;
                }

                var responder = new UdpDiscoveryResponder(listener, result.RemoteEndPoint);
                MessageReceived?.Invoke(this, new DiscoveryRawMessageReceivedEventArgs(payload, responder));
            }
            catch (OperationCanceledException) { break; }
            catch (ObjectDisposedException) { break; }
            catch (Exception ex)
            {
                logger.Log($"UDP discovery listen error: {ex.Message}");
            }
        }
    }

    /// <inheritdoc />
    public async Task BroadcastAsync(byte[] data, TimeSpan timeout, CancellationToken ct = default)
    {
        using var client = new UdpClient();
        client.EnableBroadcast = true;

        var packet = UdpPacketHelper.Pack(data);
        await client.SendAsync(packet, packet.Length, new IPEndPoint(IPAddress.Broadcast, port));

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(timeout);

        try
        {
            while (!timeoutCts.Token.IsCancellationRequested)
            {
                var result = await client.ReceiveAsync(timeoutCts.Token);
                if (!UdpPacketHelper.TryUnpack(result.Buffer, out var payload))
                {
                    continue;
                }

                var responder = new UdpDiscoveryResponder(client, result.RemoteEndPoint);
                MessageReceived?.Invoke(this, new DiscoveryRawMessageReceivedEventArgs(payload, responder));
            }
        }
        catch (OperationCanceledException) { }
    }

    /// <inheritdoc />
    public Task StopListeningAsync()
    {
        if (!isListening)
        {
            return Task.CompletedTask;
        }

        listenerCts?.Cancel();
        listener?.Dispose();
        listener = null;
        isListening = false;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await StopListeningAsync();
        listenerCts?.Dispose();
    }
}
