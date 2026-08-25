using MIN.Core.Entities.Contracts.Enums;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Events.Events;
using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.FileTransfer.Services.Contracts.Models.Enums;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.FileTransfer.Handlers;

internal sealed class FileTransferCancelHandler : BaseHandler
{
    private readonly IEventBus eventBus;
    private readonly IFileTransferService fileTransferService;

    public FileTransferCancelHandler(
        IEventBus eventBus,
        IFileTransferService fileTransferService,
        ILoggerProvider logger) : base(logger)
    {
        this.eventBus = eventBus;
        this.fileTransferService = fileTransferService;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.FileTransferCancel];

    protected override async Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var cancel = (FileTransferCancelMessage)message;

        LogInfo($"Получена отмена transfer {cancel.TransferId}: {cancel.Reason ?? "без причины"}");

        if (fileTransferService.TryGetTransferInfo(cancel.TransferId, out var info))
        {
            if (info.Direction == FileTransferDirection.Upload && context.Role == Role.Client)
            {
                context.RoomContext.Messages.RemoveMessage(info.FileMetadataId);

                await eventBus.PublishAsync(new MessageDeletedEvent
                {
                    RoomId = info.RoomId,
                    MessageId = info.FileMetadataId,
                });
            }

            await eventBus.PublishAsync(new FileTransferFailedEvent
            {
                RoomId = info.RoomId,
                FileMetadataId = info.FileMetadataId,
                SenderId = message.SenderId,
                ErrorMessage = cancel.Reason ?? "Передача отменена",
            });
        }

        LogInfo($"Удаляю transfer {cancel.TransferId} из активных");
        fileTransferService.RemoveTransfer(cancel.TransferId);

        return HandlerResult.Success();
    }
}
