using System.IO.Pipes;
using MIN.Discovery.Services.Contracts.Exceptions;
using MIN.Discovery.Transport.Contracts.Events;
using MIN.Discovery.Transport.NamedPipes.Services;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Discovery.Transport.NamedPipes.Client;

/// <summary>
/// Клиент Named Pipe для отправки данных о комнате 
/// </summary>
internal sealed class NamedPipeDiscoveryClient
{
    private readonly ILoggerProvider logger;
    private const int MaxAttempts = 3;

    /// <summary>
    /// Событие получения сырых данных от транспорта
    /// </summary>
    public event EventHandler<DiscoveryRawMessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="NamedPipeDiscoveryClient"/>
    /// </summary>
    public NamedPipeDiscoveryClient(ILoggerProvider logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Отправить данные к компьютеру
    /// </summary>
    public async Task<bool> SendAsync(byte[] data, string? machineName, TimeSpan? timeout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(machineName);

        timeout = timeout ?? TimeSpan.FromSeconds(1);

        Exception? lastException = null;

        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                await ConnectAndSendAsync(data, machineName, timeout.Value, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                lastException = ex;
                logger.Log($"[DEBUG]: attempt {attempt}/{MaxAttempts} failed for {machineName}: {ex.Message}");

                if (attempt < MaxAttempts)
                {
                    try
                    {
                        await Task.Delay((int)timeout.Value.TotalMilliseconds + ((int)timeout.Value.TotalMilliseconds * (attempt - 1) / 10), cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return false;
                    }
                }
            }
        }

        throw new DiscoveryException($"Failed to connect to {machineName} after {MaxAttempts} attempts: {lastException?.Message}");
    }

    private async Task ConnectAndSendAsync(byte[] data, string machineName, TimeSpan timeout, CancellationToken cancellationToken)
    {
        using var pipe = new NamedPipeClientStream(
            machineName,
            DiscoveryPipeNameProvider.GetDiscoveryPipeName(machineName),
            PipeDirection.InOut,
            PipeOptions.Asynchronous | PipeOptions.WriteThrough
        );

        logger.Log($"[DEBUG]: starting connect to {machineName}");

        await pipe.ConnectAsync((int)timeout.TotalMilliseconds, cancellationToken);

        logger.Log($"[DEBUG]: connected to {machineName}");

        await pipe.WriteAsync(data.AsMemory(), cancellationToken);
        await pipe.FlushAsync(cancellationToken);

        await AcceptResponse(pipe, cancellationToken);
    }

    private async Task AcceptResponse(NamedPipeClientStream pipe, CancellationToken cancellationToken)
    {
        try
        {
            var buffer = new byte[64 * 1024];
            var bytesRead = await pipe!.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
            if (bytesRead > 0)
            {
                var data = new byte[bytesRead];
                Array.Copy(buffer, data, bytesRead);
                MessageReceived?.Invoke(this, new DiscoveryRawMessageReceivedEventArgs(data));
            }
        }
        finally
        {
            pipe?.Dispose();
        }
    }
}
