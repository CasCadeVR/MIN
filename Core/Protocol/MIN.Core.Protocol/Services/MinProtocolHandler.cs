using System.Text;
using System.Text.Json;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Protocol.Contracts.Interfaces;
using MIN.Core.Protocol.Contracts.Models;
using MIN.Core.Transport.Contracts.Events;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Core.Protocol.Services;

/// <inheritdoc cref="IProtocolHandler"/>
public sealed class MinProtocolHandler : IProtocolHandler
{
    private const string ResponseStarter = "MIN ";
    private readonly ITransport transport;
    private readonly IVersionProvider versionProvider;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MinProtocolHandler"/>
    /// </summary>
    public MinProtocolHandler(ITransport transport,
        IVersionProvider versionProvider,
        ILoggerProvider logger)
    {
        this.transport = transport;
        this.versionProvider = versionProvider;
        this.logger = logger;
    }

    async Task<PreambleResult> IProtocolHandler.HandleClientAsync(Guid connectionId, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<PreambleResult>();

        logger.Log($"Protocol client: отправляю запрос на соединение {connectionId}");

        void Handler(object? sender, RawMessageReceivedEventArgs e)
        {
            if (e.ConnectionId != connectionId)
            {
                return;
            }
            transport.RawMessageReceived -= Handler;

            var response = Encoding.UTF8.GetString(e.Data);
            if (!response.StartsWith(ResponseStarter))
            {
                logger.Log($"Protocol client: неверный ответ от {connectionId}: {response[..Math.Min(response.Length, 20)]}");
                tcs.TrySetResult(new PreambleResult { IsSuccess = false, ErrorMessage = "Not a MIN endpoint" });
                return;
            }

            var roomInfo = JsonSerializer.Deserialize<RoomInfo>(response.AsSpan(4));

            if (roomInfo == null)
            {
                logger.Log($"Protocol client: не удалось десериализовать RoomInfo от {connectionId}");
                tcs.TrySetResult(new PreambleResult { IsSuccess = false, ErrorMessage = "Couldn't deserialize room information" });
                return;
            }

            logger.Log($"Protocol client: успех, комната {roomInfo.Id} ({roomInfo.Name})");
            tcs.TrySetResult(new PreambleResult { IsSuccess = true, RoomInfo = roomInfo! });
        }

        transport.RawMessageReceived += Handler;

        var request = Encoding.UTF8.GetBytes("MIN " + versionProvider.Version);
        await transport.SendAsync(request, connectionId, null, cancellationToken);

        try
        {
            var timeout = TimeSpan.FromSeconds(5);
            var result = await tcs.Task.WaitAsync(timeout, cancellationToken);
            return result;
        }
        catch (TimeoutException)
        {
            transport.RawMessageReceived -= Handler;
            logger.Log($"Protocol client: таймаут ожидания ответа от {connectionId}");
            return new PreambleResult { IsSuccess = false, ErrorMessage = "Protocol timeout" };
        }
    }

    async Task<PreambleResult> IProtocolHandler.HandleServerAsync(Guid serverConnectionId, Guid clientConnectionId, RoomInfo roomInfo, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<PreambleResult>();

        logger.Log($"Protocol server: ожидаю запрос от {clientConnectionId}");

        async void Handler(object? sender, RawMessageReceivedEventArgs e)
        {
            if (e.ConnectionId != clientConnectionId)
            {
                return;
            }
            transport.RawMessageReceived -= Handler;

            var request = Encoding.UTF8.GetString(e.Data);
            if (!request.StartsWith(ResponseStarter))
            {
                logger.Log($"Protocol server: неверный протокол от {clientConnectionId}");
                tcs.TrySetResult(new PreambleResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid protocol"
                });
                return;
            }

            var clientVersion = request[4..];

            if (!Version.TryParse(clientVersion, out var parsedVersion) || !versionProvider.IsVersionCompatible(parsedVersion))
            {
                logger.Log($"Protocol server: несовместимая версия {clientVersion} от {clientConnectionId}");
                tcs.TrySetResult(new PreambleResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Incompatible version: {clientVersion}"
                });
                return;
            }

            logger.Log($"Protocol server: клиент {clientConnectionId} прошёл протокол, версия {clientVersion}");

            var roomJson = JsonSerializer.Serialize(roomInfo);
            var response = Encoding.UTF8.GetBytes("MIN " + roomJson);
            await transport.SendAsync(response, clientConnectionId, serverConnectionId, cancellationToken);

            tcs.TrySetResult(new PreambleResult { IsSuccess = true });
        }

        transport.RawMessageReceived += Handler;

        try
        {
            return await tcs.Task.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);
        }
        catch (TimeoutException)
        {
            transport.RawMessageReceived -= Handler;
            logger.Log($"Protocol server: таймаут ожидания запроса от {clientConnectionId}");
            return new PreambleResult { IsSuccess = false, ErrorMessage = "Protocol timeout" };
        }
    }
}
