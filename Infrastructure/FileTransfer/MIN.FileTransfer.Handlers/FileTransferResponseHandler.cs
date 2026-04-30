using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.Core.Streaming.Contracts.Models;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.FileTransfer.Services.Contracts.Models.Enums;

namespace MIN.FileTransfer.Handlers;

internal sealed class FileTransferResponseHandler : IMessageHandler, IFileTransferHandlerAnchor
{
    private readonly IEventBus eventBus;
    private readonly IFileTransferService fileTransferService;
    private readonly IMessageSender messageSender;
    private readonly IStreamManager streamManager;

    public FileTransferResponseHandler(
        IEventBus eventBus,
        IFileTransferService fileTransferService,
        IMessageSender messageSender,
        IStreamManager streamManager)
    {
        this.eventBus = eventBus;
        this.fileTransferService = fileTransferService;
        this.messageSender = messageSender;
        this.streamManager = streamManager;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.FileTransferResponse];

    int IMessageHandler.Priority => 5;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not FileTransferResponseMessage response)
        {
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(FileTransferResponseHandler)} - {message.GetType()}");
        }

        if (!response.Success)
        {
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

        if (!fileTransferService.TryGetTransferInfo(response.TransferId, out var transferInfo))
        {
            return HandlerResult.Failure($"Не найдена информация о transfer {response.TransferId}", stopPropagation: false);
        }

        if (transferInfo.Direction == FileTransferDirection.Upload)
        {
            var filePath = fileTransferService.GetFilePath(transferInfo.RoomId, transferInfo.FileName);
            if (filePath == null)
            {
                var errorResponse = new FileTransferResponseMessage
                {
                    RoomId = transferInfo.RoomId,
                    TransferId = response.TransferId,
                    Success = false,
                    ErrorMessage = "File not found on server",
                };

                await messageSender.SendAsync(errorResponse, transferInfo.RoomId, context.ConnectionId, context.CancellationToken);

                return HandlerResult.Failure("File not found on server", stopPropagation: true);
            }

            await using var fileStream = File.OpenRead(filePath);
            var fileBytes = new byte[fileStream.Length];
            await fileStream.ReadAsync(fileBytes, context.CancellationToken);

            var options = new StreamOptions
            {
                RequiresAcks = true,
                RequiresEncryption = true,
            };

            await streamManager.SendAsync(fileBytes, options, transferInfo.RoomId, context.ConnectionId, context.CancellationToken);
        }

        return HandlerResult.Success(stopPropagation: true);
    }
}
