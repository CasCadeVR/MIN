using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.FileTransfer.Handlers;

internal sealed class FileTransferCancelHandler : IMessageHandler, IFileTransferHandlerAnchor
{
    private readonly IEventBus eventBus;
    private readonly IFileTransferService fileTransferService;
    private readonly ILoggerProvider logger;

    public FileTransferCancelHandler(
        IEventBus eventBus,
        IFileTransferService fileTransferService,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.fileTransferService = fileTransferService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.FileTransferCancel];

    int IMessageHandler.Priority => 5;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not FileTransferCancelMessage cancel)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(FileTransferCancelHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(FileTransferCancelHandler)} - {message.GetType()}");
        }

        logger.Log($"Получена отмена transfer {cancel.TransferId}: {cancel.Reason ?? "без причины"}");

        if (fileTransferService.TryGetTransferInfo(cancel.TransferId, out var info))
        {
            logger.Log($"Transfer {cancel.TransferId} принадлежит комнате {info.RoomId}, файл: {info.FileName}");

            await eventBus.PublishAsync(new FileTransferFailedEvent
            {
                RoomId = info.RoomId,
                TransferId = cancel.TransferId,
                ErrorMessage = cancel.Reason ?? "Передача не удалась",
            });
        }

        logger.Log($"Удаляю transfer {cancel.TransferId} из активных");
        fileTransferService.RemoveTransfer(cancel.TransferId);

        return HandlerResult.Success();
    }
}
