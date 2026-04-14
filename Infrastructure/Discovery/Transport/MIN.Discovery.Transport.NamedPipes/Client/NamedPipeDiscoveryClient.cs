using System.IO.Pipes;
using System.Net.NetworkInformation;
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
    public async Task SendAsync(byte[] data, string? machineName, TimeSpan? timeout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(machineName);

        timeout = timeout ?? TimeSpan.FromSeconds(1);

        await PingAsync(machineName, timeout.Value, cancellationToken);

        logger.Log($"Отправляю запрос discovery к {machineName}");

        using var pipe = new NamedPipeClientStream(
            machineName,
            DiscoveryPipeNameProvider.GetDiscoveryPipeName(machineName),
            PipeDirection.InOut,
            PipeOptions.Asynchronous | PipeOptions.WriteThrough
        );

        try
        {
            await pipe.ConnectAsync(timeout.Value.Seconds, cancellationToken);

            // Connected here

            await pipe.WriteAsync(data.AsMemory(), cancellationToken);
            await pipe.FlushAsync(cancellationToken);

            await AcceptResponse(pipe, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new DiscoveryException(ex.Message);
        }
    }

    private async Task AcceptResponse(NamedPipeClientStream pipe, CancellationToken cancellationToken)
    {
        try
        {
            var buffer = new byte[4096];
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

    private static async Task PingAsync(string host, TimeSpan timeout, CancellationToken cancellationToken)
    {
        try
        {
            using var pingSender = new Ping();
            var pingTask = pingSender.SendPingAsync(host, timeout, cancellationToken: cancellationToken);

            if (await Task.WhenAny(pingTask, Task.Delay(timeout, cancellationToken)) == pingTask)
            {
                var reply = await pingTask;
                if (reply.Status == IPStatus.Success)
                {
                    return;
                }
                throw new DiscoveryException(reply.Status.ToString());
            }
            throw new DiscoveryException("Истекло время проверки хоста");
        }
        catch (Exception ex) when (ex is not DiscoveryException)
        {
            throw new DiscoveryException(ex.Message);
        }
    }
}
