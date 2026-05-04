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

internal sealed class FileTransferResponseHandler : IMessageHandler, IFileTransferHandlerAnchor
{
    private readonly IEventBus eventBus;
    private readonly IFileTransferService fileTransferService;
    private readonly ILoggerProvider logger;

    public FileTransferResponseHandler(
        IEventBus eventBus,
        IFileTransferService fileTransferService,
        ILoggerProvider logger)
    {
        this.eventBus = eventBus;
        this.fileTransferService = fileTransferService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.FileTransferResponse];

    int IMessageHandler.Priority => 5;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not FileTransferResponseMessage response)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(FileTransferResponseHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(FileTransferResponseHandler)} - {message.GetType()}");
        }

        logger.Log($"Получен FileTransferResponse: TransferId={response.TransferId}, Success={response.Success}");

        if (!response.Success)
        {
            logger.Log($"Transfer {response.TransferId} завершился ошибкой: {response.ErrorMessage ?? "Unknown error"}");

            if (fileTransferService.TryGetTransferInfo(response.TransferId, out var info))
            {
                await eventBus.PublishAsync(new FileTransferFailedEvent
                {
                    RoomId = info.RoomId,
                    TransferId = response.TransferId,
                    ErrorMessage = response.ErrorMessage ?? "Unknown error",
                });

                fileTransferService.RemoveTransfer(response.TransferId);
            }

            return HandlerResult.Failure(response.ErrorMessage ?? "File transfer failed", stopPropagation: false);
        }

        if (!fileTransferService.TryGetTransferInfo(response.TransferId, out _))
        {
            logger.Log($"Не найдена информация о transfer {response.TransferId}, но Response успешен — ожидаю чанки");
        }
        else
        {
            logger.Log($"Transfer {response.TransferId} подтверждён, ожидаю чанки файла");
        }

        return HandlerResult.Success(stopPropagation: true);
    }
}
