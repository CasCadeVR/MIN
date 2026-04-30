using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;

namespace MIN.FileTransfer.Handlers;

internal sealed class FileTransferCancelHandler : IMessageHandler, IFileTransferHandlerAnchor
{
    private readonly IEventBus eventBus;
    private readonly IFileTransferService fileTransferService;

    public FileTransferCancelHandler(
        IEventBus eventBus,
        IFileTransferService fileTransferService)
    {
        this.eventBus = eventBus;
        this.fileTransferService = fileTransferService;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.FileTransferCancel];

    int IMessageHandler.Priority => 5;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not FileTransferCancelMessage cancel)
        {
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(FileTransferCancelHandler)} - {message.GetType()}");
        }

        if (fileTransferService.TryGetTransferInfo(cancel.TransferId, out var info))
        {
            await eventBus.PublishAsync(new FileTransferFailedEvent
            {
                RoomId = info.RoomId,
                TransferId = cancel.TransferId,
                ErrorMessage = cancel.Reason ?? "Transfer cancelled",
            });
        }

        fileTransferService.RemoveTransfer(cancel.TransferId);

        return HandlerResult.Success();
    }
}
