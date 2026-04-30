using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.FileTransfer.Services.Contracts.Models.Enums;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.FileTransfer.Handlers;

internal sealed class FileMetadataHandler : IMessageHandler, IFileTransferHandlerAnchor
{
    private readonly IIdentityService identityService;
    private readonly IEventBus eventBus;
    private readonly IFileTransferService fileTransferService;
    private readonly IMessageSender messageSender;

    public FileMetadataHandler(
        IIdentityService identityService,
        IEventBus eventBus,
        IFileTransferService fileTransferService,
        IMessageSender messageSender)
    {
        this.identityService = identityService;
        this.eventBus = eventBus;
        this.fileTransferService = fileTransferService;
        this.messageSender = messageSender;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.FileMetadata];

    int IMessageHandler.Priority => 5;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not FileMetadataMessage metadata)
        {
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(FileMetadataHandler)} - {message.GetType()}");
        }

        var selfId = identityService.SelfParticipant.Id;

        if (metadata.SenderId == selfId)
        {
            return HandlerResult.Success();
        }

        if (!context.RoomContext.Participants.TryGetParticipantById(metadata.SenderId, out var sender))
        {
            return HandlerResult.Failure("Получил метаданные файла от неизвестного отправителя", stopPropagation: false);
        }

        fileTransferService.RegisterPendingMetadata(metadata.TransferId, metadata.FileName);

        fileTransferService.RegisterTransfer(metadata.TransferId, metadata.RoomId, FileTransferDirection.Download, metadata.FileName);

        var requestMessage = new FileTransferRequestMessage
        {
            RoomId = metadata.RoomId,
            TransferId = metadata.TransferId,
            Direction = FileTransferDirection.Upload,
            RecipientId = metadata.SenderId,
        };

        await messageSender.SendAsync(requestMessage, metadata.RoomId, context.ConnectionId, context.CancellationToken);

        await eventBus.PublishAsync(new FileTransferStartedEvent
        {
            RoomId = metadata.RoomId,
            TransferId = metadata.TransferId,
            FileName = metadata.FileName,
            FileSize = metadata.FileSize,
            Direction = FileTransferDirection.Download,
        });

        return HandlerResult.Success(stopPropagation: true);
    }
}
