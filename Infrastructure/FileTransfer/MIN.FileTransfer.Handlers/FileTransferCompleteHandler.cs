using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;

namespace MIN.FileTransfer.Handlers;

internal sealed class FileTransferCompleteHandler : IMessageHandler, IFileTransferHandlerAnchor
{
    private readonly IFileTransferService fileTransferService;

    public FileTransferCompleteHandler(IFileTransferService fileTransferService)
    {
        this.fileTransferService = fileTransferService;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.FileTransferComplete];

    int IMessageHandler.Priority => 10;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not FileTransferCompleteMessage complete)
        {
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(FileTransferCompleteHandler)} - {message.GetType()}");
        }

        fileTransferService.RemoveTransfer(complete.TransferId);

        return HandlerResult.Success();
    }
}
