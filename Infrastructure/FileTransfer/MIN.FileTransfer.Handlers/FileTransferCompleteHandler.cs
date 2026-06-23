using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.FileTransfer.Handlers;

internal sealed class FileTransferCompleteHandler : IMessageHandler
{
    private readonly IFileTransferService fileTransferService;
    private readonly ILoggerProvider logger;

    public FileTransferCompleteHandler(IFileTransferService fileTransferService, ILoggerProvider logger)
    {
        this.fileTransferService = fileTransferService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.FileTransferComplete];

    int IMessageHandler.Priority => 10;

    Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not FileTransferCompleteMessage complete)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(FileTransferCompleteHandler)} - {message.GetType()}");
            return Task.FromResult(HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(FileTransferCompleteHandler)} - {message.GetType()}"));
        }

        logger.Log($"Transfer {complete.TransferId} завершён, очищаю информацию");
        fileTransferService.RemoveTransfer(complete.TransferId);

        return Task.FromResult(HandlerResult.Success());
    }
}
