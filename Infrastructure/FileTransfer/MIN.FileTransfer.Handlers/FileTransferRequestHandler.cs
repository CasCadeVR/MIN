using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.FileTransfer.Services.Contracts.Models.Enums;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.FileTransfer.Handlers;

internal sealed class FileTransferRequestHandler : IMessageHandler, IFileTransferHandlerAnchor
{
    private readonly IIdentityService identityService;
    private readonly IFileTransferService fileTransferService;
    private readonly IMessageSender messageSender;

    public FileTransferRequestHandler(
        IIdentityService identityService,
        IFileTransferService fileTransferService,
        IMessageSender messageSender)
    {
        this.identityService = identityService;
        this.fileTransferService = fileTransferService;
        this.messageSender = messageSender;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.FileTransferRequest];

    int IMessageHandler.Priority => 5;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not FileTransferRequestMessage request)
        {
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(FileTransferRequestHandler)} - {message.GetType()}");
        }

        var selfId = identityService.SelfParticipant.Id;

        if (request.Direction == FileTransferDirection.Upload)
        {
            if (request.RecipientId != selfId)
            {
                return HandlerResult.Failure("Запрос на загрузку файла адресован не мне", stopPropagation: false);
            }

            if (!fileTransferService.TryGetTransferInfo(request.TransferId, out var info))
            {
                return HandlerResult.Failure($"Не найдена информация о transfer {request.TransferId}", stopPropagation: false);
            }

            var response = new FileTransferResponseMessage
            {
                RoomId = request.RoomId,
                TransferId = request.TransferId,
                Success = true,
            };

            await messageSender.SendAsync(response, request.RoomId, context.ConnectionId, context.CancellationToken);

            return HandlerResult.Success(stopPropagation: true);
        }
        else
        {
            if (!fileTransferService.TryGetTransferInfo(request.TransferId, out _))
            {
                fileTransferService.RegisterTransfer(request.TransferId, request.RoomId, FileTransferDirection.Download, "");
            }

            var response = new FileTransferResponseMessage
            {
                RoomId = request.RoomId,
                TransferId = request.TransferId,
                Success = true,
            };

            await messageSender.SendAsync(response, request.RoomId, context.ConnectionId, context.CancellationToken);

            return HandlerResult.Success(stopPropagation: true);
        }
    }
}
