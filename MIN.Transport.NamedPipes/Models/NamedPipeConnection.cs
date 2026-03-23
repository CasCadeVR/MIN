using System.IO.Pipes;
using MIN.Messaging.Contracts.Entities;

namespace MIN.Transport.NamedPipes.Models;

/// <summary>
/// Соединение через Named Pipe
/// </summary>
internal sealed class NamedPipeConnection : IAsyncDisposable
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private bool disposed;

    /// <summary>
    /// Инициализирует новый экзмепляр <see cref="NamedPipeConnection"/>
    /// </summary>
    public NamedPipeConnection(PipeStream pipe, NamedPipeEndpoint endpoint, ParticipantInfo? participant = null)
    {
        Pipe = pipe;
        Endpoint = endpoint;
        Participant = participant;
    }

    /// <summary>
    /// Идентификатор соединения
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Точка подключения
    /// </summary>
    public NamedPipeEndpoint Endpoint { get; } = null!;

    /// <summary>
    /// Pipe подключения
    /// </summary>
    public PipeStream Pipe { get; } = null!;

    /// <summary>
    /// Участник, связанный с этим соединением
    /// </summary>
    public ParticipantInfo? Participant { get; set; }

    /// <summary>
    /// Активно ли соединение
    /// </summary>
    public bool IsConnected => Pipe.IsConnected && !disposed;

    /// <summary>
    /// Событие получения сообщения
    /// </summary>
    public event EventHandler<byte[]>? RawMessageReceived;

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
                    var bytesRead = await Pipe.ReadAsync(buffer.AsMemory(0, buffer.Length), linkedCts.Token);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    var data = new byte[bytesRead];
                    Array.Copy(buffer, data, bytesRead);
                    OnRawMessageReceived(data);
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

        await Pipe.WriteAsync(data.AsMemory(), cancellationToken);
        await Pipe.FlushAsync(cancellationToken);
    }

    private void OnRawMessageReceived(byte[] data)
    {
        RawMessageReceived?.Invoke(this, data);
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
            if (Pipe.IsConnected)
            {
                await Pipe.DisposeAsync();
            }
        }
        catch
        {
            // ignore
        }

        cancellationTokenSource.Dispose();
    }
}
