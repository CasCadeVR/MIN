using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Dispatcher;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Moderation;
using MIN.Core.Stores.Contracts.Registries.Models;
using MIN.Core.SubRooms.Contracts.Interfaces;
using MIN.Core.SubRooms.Contracts.Interfaces.Messages;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;

namespace MIN.Core.Handlers.Dispatcher;

/// <inheritdoc cref="IMessageDispatcher"/>
public sealed class MessageDispatcher : IMessageDispatcher
{
    private readonly IEnumerable<IMessageHandler> handlers;
    private readonly IMessageSender messageSender;
    private readonly IEventBus eventBus;
    private readonly ISubRoomManager subRoomManager;
    private readonly INetworkErrorHandler errorHandler;
    private readonly ILoggerProvider logger;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MessageDispatcher"/>
    /// </summary>
    public MessageDispatcher(IEnumerable<IMessageHandler> handlers,
        IMessageSender messageSender,
        IEventBus eventBus,
        ISubRoomManager subRoomManager,
        INetworkErrorHandler errorHandler,
        ILoggerProvider logger)
    {
        this.handlers = handlers;
        this.messageSender = messageSender;
        this.eventBus = eventBus;
        this.subRoomManager = subRoomManager;
        this.errorHandler = errorHandler;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task DispatchAsync(IMessage message, MessageContext context, IEnumerable<Guid>? broadcastExcludeIds)
    {
        var applicableHandlers = handlers
            .Where(h => h.HandledTypes.Contains(message.TypeTag))
            .OrderBy(h => h.Priority)
            .ToList();

        if (applicableHandlers.Count == 0)
        {
            throw new NotImplementedException($"Не зарегистрирован обработчик для {message.TypeTag}");
        }

        foreach (var handler in applicableHandlers)
        {
            try
            {
                if (broadcastExcludeIds?.Contains(context.SelfId) == true && context.Role == Role.Host)
                {
                    await HandleServerMessageRouting(message, context, broadcastExcludeIds);
                    continue;
                }

                var result = await handler.HandleAsync(message, context);

                if (!result.IsSuccess)
                {
                    logger.Log($"Обработчик {handler.GetType().Name} провалился: {result.ErrorMessage}", LogLevel.Error);
                    if (result.ShowErrorMessage)
                    {
                        await PublishErrorEvent(result.ErrorMessage ?? "Неизвестная ошибка", result.CriticalError, context);
                    }
                    continue;
                }

                if (result.Response != null)
                {
                    result.Response.SenderId = context.SelfId;
                    if (context.ConnectionId == CoreRegistryConstants.LocalConnectionId)
                    {
                        await DispatchAsync(result.Response, context, broadcastExcludeIds);
                    }
                    else
                    {
                        await messageSender.SendAsync(result.Response, context.RoomContext.RoomId, context.ConnectionId, context.CancellationToken);
                    }
                }

                if (result.ResultEvent != null)
                {
                    await eventBus.PublishAsync(result.ResultEvent, context.CancellationToken);
                }

                if (result.ErrorMessage != null)
                {
                    if (context.Role == Role.Host)
                    {
                        await PublishErrorEvent(result.ErrorMessage, needToDisconnect: false, context);
                    }
                    else
                    {
                        await errorHandler.SendErrorToConnectionAsync(result.ErrorMessage, context.ConnectionId, context.RoomContext.RoomId, result.CriticalError);
                    }
                    continue;
                }

                if (result.StopPropagation)
                {
                    break;
                }

                if (context.Role == Role.Host)
                {
                    await HandleServerMessageRouting(message, context, broadcastExcludeIds);
                }
            }
            catch (Exception ex)
            {
                logger.Log($"Handler {handler.GetType().Name} threw exception: {ex.Message}", LogLevel.Error);
                await PublishErrorEvent(ex.Message, needToDisconnect: true, context);
            }
        }
    }

    private async Task HandleServerMessageRouting(IMessage message, MessageContext context, IEnumerable<Guid>? broadcastExcludeIds)
    {
        if (message.IsPublic)
        {
            var roomParticipantsIds = context.RoomContext.Participants.GetParticipants().Select(x => x.Id);
            var senderConnectionId = context.RoomContext.Connections.GetConnectionIdFromParticipantId(message.SenderId);

            var excludeConnectionIds = new List<Guid>
            {
                senderConnectionId
            }.Concat(broadcastExcludeIds?.Where(roomParticipantsIds.Contains).Select(context.RoomContext.Connections.GetConnectionIdFromParticipantId) ?? []).ToList();

            if (message is IWithinSubRoom withinSubRoomMessage)
            {
                var subRoomParticipants = subRoomManager.GetParticipantIds(context.RoomContext.RoomId, withinSubRoomMessage.SubRoomId);
                excludeConnectionIds.AddRange(roomParticipantsIds.Except(subRoomParticipants)
                    .Select(context.RoomContext.Connections.GetConnectionIdFromParticipantId));
            }

            try
            {
                await messageSender.BroadcastAsync(message, context.RoomContext.RoomId, excludeConnectionIds, context.CancellationToken);
            }
            catch (OperationCanceledException)
            {
                logger.Log($"Широковещательная отправка в комнате {context.RoomContext.RoomId} прервалась", LogLevel.Warning);
            }
        }
        else if (message.RecipientId != null)
        {
            if (!context.RoomContext.Connections.TryGetConnectionIdFromParticipantId(message.RecipientId ?? Guid.Empty, out var recipientConnectionId))
            {
                logger.Log($"Не удалось найти участника с id {message.RecipientId} во время маршрутизации приватного сообщения", LogLevel.Error);
                return;
            }

            if (recipientConnectionId != CoreRegistryConstants.LocalConnectionId)
            {
                try
                {
                    await messageSender.SendAsync(message, context.RoomContext.RoomId, recipientConnectionId, context.CancellationToken);
                }
                catch (OperationCanceledException)
                {
                    logger.Log($"Отправка приватного сообщения в комнате {context.RoomContext.RoomId} прервалась", LogLevel.Warning);
                }
            }
        }
    }

    private async Task PublishErrorEvent(string message, bool needToDisconnect, MessageContext context)
        => await eventBus.PublishAsync(new ErrorOccurredEvent()
        {
            ErrorMessage = message,
            NeedToDisconnect = needToDisconnect,
            RoomId = context.RoomContext.RoomId
        }, context.CancellationToken);
}
