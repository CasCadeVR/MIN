using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Events.Contracts;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.FileTransfer.Services.Contracts.Models;
using MIN.FileTransfer.Services.Contracts.Models.Enums;
using MIN.Helpers.Contracts.Extensions;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatFileService"/>
public sealed class ChatFileService : IChatFileService
{
    private readonly IMessageRouter messageRouter;
    private readonly IRoomHoster roomHoster;
    private readonly IEventBus eventBus;
    private readonly IFileHelperService fileHelperService;
    private readonly IFileTransferService fileTransferService;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFileService"/>
    /// </summary>
    public ChatFileService(IMessageRouter messageRouter,
        IRoomHoster roomHoster,
        IEventBus eventBus,
        IFileHelperService fileHelperService,
        IFileTransferService fileTransferService,
        IIdentityService identityService)
    {
        this.messageRouter = messageRouter;
        this.roomHoster = roomHoster;
        this.eventBus = eventBus;
        this.fileHelperService = fileHelperService;
        this.fileTransferService = fileTransferService;
        this.identityService = identityService;
    }

    async Task IChatFileService.SendFileAsync(Guid roomId, string fileName, string filePath, Guid? recipientId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !Path.Exists(filePath))
        {
            throw new ArgumentException("Файл не найден", nameof(filePath));
        }

        var transferId = Guid.NewGuid();

        var message = new FileMetadataMessage
        {
            TransferId = transferId,
            RoomId = roomId,
            Sender = identityService.SelfParticipant.ToParticipantInfo(),
            FileName = fileName,
            SenderId = identityService.SelfParticipant.Id,
            FilePath = filePath,
            FileSize = fileHelperService.GetFileSize(filePath),
            RecipientId = recipientId,
        };

        if (!roomHoster.IsHosting(roomId))
        {
            // Ожидаем, что хост запросит с нас файл
            var info = new TransferInfo
            {
                TransferId = transferId,
                FileMetadataId = message.Id,
                RoomId = roomId,
                SenderId = identityService.SelfParticipant.Id,
                Direction = FileTransferDirection.Upload,
                FileName = fileName,
            };
            fileTransferService.RegisterTransfer(info);
        }

        await messageRouter.RouteAsync(message, roomId, identityService.SelfParticipant.Id, cancellationToken);
    }

    async Task IChatFileService.RequestFileDownloadAsync(Guid roomId, FileMetadataMessage fileMetadataMessage, CancellationToken cancellationToken)
    {
        var transferId = Guid.NewGuid();
        fileMetadataMessage.TransferId = transferId;

        var info = new TransferInfo
        {
            TransferId = transferId,
            FileMetadataId = fileMetadataMessage.Id,
            RoomId = roomId,
            SenderId = identityService.SelfParticipant.Id,
            Direction = FileTransferDirection.Download,
            FileName = fileMetadataMessage.FileName,
        };

        fileTransferService.RegisterTransfer(info);

        await eventBus.PublishAsync(new FileTransferStartedEvent
        {
            RoomId = fileMetadataMessage.RoomId,
            TransferId = fileMetadataMessage.TransferId,
            FileMetadataId = fileMetadataMessage.Id,
            FileName = fileMetadataMessage.FileName,
            FileSize = fileMetadataMessage.FileSize,
            Sender = fileMetadataMessage.Sender,
            Direction = FileTransferDirection.Download,
        }, cancellationToken);

        var message = new FileTransferRequestMessage
        {
            TransferId = transferId,
            RoomId = roomId,
            FileName = fileMetadataMessage.FileName,
            FileMetadataId = fileMetadataMessage.Id,
            Direction = FileTransferDirection.Download,
        };

        await messageRouter.RouteAsync(message, roomId, identityService.SelfParticipant.Id, cancellationToken);
    }

    async Task IChatFileService.CancelFileDownloadAsync(Guid roomId, FileMetadataMessage fileMessage, CancellationToken cancellationToken)
    {
        var message = new FileTransferCancelMessage
        {
            TransferId = fileMessage.TransferId,
            RoomId = roomId,
            FileMetadataId = fileMessage.Id,
        };

        await messageRouter.RouteAsync(message, roomId, identityService.SelfParticipant.Id, cancellationToken);
    }
}
