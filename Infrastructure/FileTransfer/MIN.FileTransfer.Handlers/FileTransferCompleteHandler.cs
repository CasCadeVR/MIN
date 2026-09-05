using MIN.Core.Handlers.Contracts.Base;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.FileTransfer.Handlers;

internal sealed class FileTransferCompleteHandler : BaseHandler
{
    private readonly IFileTransferService fileTransferService;

    public FileTransferCompleteHandler(IFileTransferService fileTransferService,
        ILoggerProvider logger) : base(logger)
    {
        this.fileTransferService = fileTransferService;
    }

    public override IEnumerable<MessageTypeTag> HandledTypes => [MessageTypeTag.FileTransferComplete];

    protected override Task<HandlerResult> HandleAsync(IMessage message, MessageContext context)
    {
        var complete = (FileTransferCompleteMessage)message;
        LogInfo($"Transfer {complete.TransferId} завершён, очищаю информацию");
        fileTransferService.RemoveTransfer(complete.TransferId);
        return Task.FromResult(HandlerResult.Success());
    }
}
