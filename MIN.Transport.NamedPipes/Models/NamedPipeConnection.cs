using System.IO.Pipes;
using MIN.Messaging.Contracts.Entities;
using MIN.Transport.Contracts.Endpoints;

namespace MIN.Transport.NamedPipes.Models;

/// <summary>
/// Представляет соединение через Named Pipe
/// </summary>
internal sealed class NamedPipeConnection : IAsyncDisposable
{
    private readonly PipeStream pipe;
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private bool disposed;

    public NamedPipeConnection(PipeStream pipe, NamedPipeEndpoint endpoint, ParticipantInfo? participant = null)
    {
        this.pipe = pipe;
        Endpoint = endpoint;
        Participant = participant;
    }

    /// <summary>
    /// Точка подключения
    /// </summary>
    public NamedPipeEndpoint Endpoint { get; }

    /// <summary>
    /// Pipe подключения
    /// </summary>
    public PipeStream Pipe { get; }

    /// <summary>
    /// Участник, связанный с этим соединением (устанавливается после handshake)
    /// </summary>
    public ParticipantInfo? Participant { get; set; }

    /// <summary>
    /// Идентификатор соединения (генерируется автоматически)
    /// </summary>
    public Guid ConnectionId { get; } = Guid.NewGuid();

    /// <summary>
    /// Флаг, указывающий, активно ли соединение
    /// </summary>
    public bool IsConnected => pipe.IsConnected && !disposed;

    /// <summary>
    /// Событие получения сообщения
    /// </summary>
    public event EventHandler<byte[]>? MessageReceived;

    /// <summary>
    /// Событие отключения
    /// </summary>
    public event EventHandler<string?>? Disconnected;

    /// <summary>
    /// Запускает асинхронное чтение сообщений из pipe
    /// </summary>
    public Task StartReadingAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(async () =>
        {
            var buffer = new byte[4096];
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, cancellationToken);

            try
            {
                while (!linkedCts.Token.IsCancellationRequested && IsConnected)
                {
                    var bytesRead = await pipe.ReadAsync(buffer.AsMemory(0, buffer.Length), linkedCts.Token);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    var data = new byte[bytesRead];
                    Array.Copy(buffer, data, bytesRead);
                    OnMessageReceived(data);
                }
            }
            catch (OperationCanceledException)
            {
                // normal exit
            }
            catch (Exception ex)
            {
                OnDisconnected(ex.Message);
            }
            finally
            {
                await DisposeAsync();
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Отправляет данные
    /// </summary>
    public async Task SendAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("Connection is not active");
        }

        await pipe.WriteAsync(data.AsMemory(), cancellationToken);
        await pipe.FlushAsync(cancellationToken);
    }

    private void OnMessageReceived(byte[] data)
    {
        MessageReceived?.Invoke(this, data);
    }

    private void OnDisconnected(string? reason)
    {
        Disconnected?.Invoke(this, reason);
    }

    /// <summary>
    /// Освобождает ресурсы
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        cancellationTokenSource.Cancel();

        try
        {
            if (pipe.IsConnected)
            {
                await pipe.DisposeAsync();
            }
        }
        catch
        {
            // ignore
        }

        cancellationTokenSource.Dispose();
    }
}
