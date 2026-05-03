using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.FileTransfer.Services.Contracts.Models.Enums;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.FileTransfer.Handlers;

internal sealed class FileMetadataHandler : IMessageHandler, IFileTransferHandlerAnchor
{
    private readonly IFileTransferService fileTransferService;
    private readonly IEventBus eventBus;
    private readonly IRoomHoster roomHoster;
    private readonly IIdentityService identityService;

    public FileMetadataHandler(IFileTransferService fileTransferService,
        IEventBus eventBus,
        IRoomHoster roomHoster,
        IIdentityService identityService)
    {
        this.fileTransferService = fileTransferService;
        this.eventBus = eventBus;
        this.roomHoster = roomHoster;
        this.identityService = identityService;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.FileMetadata];

    int IMessageHandler.Priority => 5;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not FileMetadataMessage metadata)
        {
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(FileMetadataHandler)} - {message.GetType()}");
        }

        if (!context.RoomContext.Participants.TryGetParticipantById(metadata.SenderId, out var sender))
        {
            return HandlerResult.Failure("Получил метаданные файла от неизвестного отправителя", stopPropagation: false);
        }

        context.RoomContext.Messages.AddMessage(metadata);
        var selfId = identityService.SelfParticipant.Id;

        if (message.SenderId != selfId)
        {
            metadata.FilePath = null;
        }

        if (message.SenderId == selfId || message.RecipientId == selfId || message.IsPublic)
        {
            await eventBus.PublishAsync(new FileMetaDataMessageReceivedEvent()
            {
                Message = metadata,
                RoomId = context.RoomContext.RoomId,
                Sender = sender!,
            });
        }

        if (!roomHoster.IsHosting(context.RoomContext.RoomId) || message.SenderId == selfId)
        {
            return HandlerResult.Success();
        }

        fileTransferService.RegisterPendingMetadata(metadata.TransferId, metadata.FileName);
        fileTransferService.RegisterTransfer(metadata.TransferId, metadata.RoomId, FileTransferDirection.Upload, metadata.FileName);

        var requestMessage = new FileTransferRequestMessage
        {
            RoomId = metadata.RoomId,
            TransferId = metadata.TransferId,
            RecipientId = metadata.SenderId,
            Direction = FileTransferDirection.Upload,
        };

        await eventBus.PublishAsync(new FileTransferStartedEvent
        {
            RoomId = metadata.RoomId,
            TransferId = metadata.TransferId,
            FileName = metadata.FileName,
            FileSize = metadata.FileSize,
            Direction = FileTransferDirection.Upload,
        });

        return HandlerResult.WithResponse(requestMessage, stopPropagation: true);
    }
}
