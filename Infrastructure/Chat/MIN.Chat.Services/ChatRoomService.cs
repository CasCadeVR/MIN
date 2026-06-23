using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Messaging.Stateless.RoomRelated.History;
using MIN.Core.Messaging.Stateless.RoomRelated.RoomInfo;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.Core.Stores.Contracts.Constants;
using MIN.Core.Stores.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatRoomService"/>
public sealed class ChatRoomService : IChatRoomService
{
    private readonly IMessageRouter messageRouter;
    private readonly IRoomFactory roomFactory;
    private readonly IRoomHoster roomHoster;
    private readonly INetworkErrorHandler networkErrorHandler;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatRoomService"/>
    /// </summary>
    public ChatRoomService(IMessageRouter messageRouter,
        IRoomFactory roomFactory,
        IRoomHoster roomHoster,
        INetworkErrorHandler networkErrorHandler,
        IIdentityService identityService)
    {
        this.messageRouter = messageRouter;
        this.roomFactory = roomFactory;
        this.roomHoster = roomHoster;
        this.networkErrorHandler = networkErrorHandler;
        this.identityService = identityService;
    }

    async Task IChatRoomService.KickParticipantAsync(Guid roomId, Guid participantId, string reason, CancellationToken cancellationToken)
    {
        roomFactory.TryGetContext(roomId, out var context);

        if (context == null)
        {
            throw new ArgumentNullException("Не нашлась информация о комнате");
        }

        if (!roomHoster.IsHosting(roomId))
        {
            throw new InvalidOperationException("Ты не являешся хостом для этой комнаты");
        }

        var kickingParticipantConnectionId = context.Connections.GetConnectionIdFromParticipantId(participantId);

        await networkErrorHandler.SendErrorAsync(reason, participantId, roomId, critical: true);
    }

    async Task IChatRoomService.SendChatHistoryRequest(Guid roomId, int page, CancellationToken cancellationToken)
    {
        var request = new ChatHistoryRequestMessage
        {
            Page = page,
            PageSize = StoreConstants.MessagesPageSize,
        };

        await messageRouter.RouteAsync(request, roomId, identityService.SelfParticipant.Id, cancellationToken);
    }

    async Task IChatRoomService.SendUpdatedRoomInfoAsync(RoomInfo updatedRoomInfo, CancellationToken cancellationToken)
    {
        var roomId = updatedRoomInfo.Id;

        if (!roomHoster.IsHosting(roomId))
        {
            throw new InvalidOperationException("Ты не являешся хостом для этой комнаты");
        }

        await messageRouter.RouteAsync(new RoomInfoUpdatedMessage
        {
            Room = updatedRoomInfo
        }, roomId, identityService.SelfParticipant.Id, cancellationToken);
    }
}
