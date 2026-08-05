using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Extensions;
using MIN.Core.Events.Contracts.Interfaces;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Stores.Contracts.Registries.Interfaces;
using MIN.FileTransfer.DI.FeatureCollection;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Models;
using MIN.FileTransfer.Services.Contracts.Models.Enums;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatFileService"/>
public sealed class ChatFileService : IChatFileService
{
    private readonly IMessageRouter messageRouter;
    private readonly IRoomConnectionRegistry registry;
    private readonly IEventBus eventBus;
    private readonly IFileTransferFeatureCollection fileFeatureCollection;
    private readonly IIdentityService identityService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatFileService"/>
    /// </summary>
    public ChatFileService(IMessageRouter messageRouter,
        IRoomConnectionRegistry registry,
        IEventBus eventBus,
        IFileTransferFeatureCollection fileFeatureCollection,
        IIdentityService identityService)
    {
        this.messageRouter = messageRouter;
        this.registry = registry;
        this.eventBus = eventBus;
        this.fileFeatureCollection = fileFeatureCollection;
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
            Sender = identityService.SelfParticipant.ToParticipantInfo(),
            FileName = fileName,
            SenderId = identityService.SelfParticipant.Id,
            FilePath = filePath,
            FileSize = fileFeatureCollection.FileHelperService.GetFileSize(filePath),
            RecipientId = recipientId,
        };

        if (!registry.IsHosting(roomId))
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
            fileFeatureCollection.FileTransferService.RegisterTransfer(info);
        }

        await messageRouter.RouteAsync(message, roomId, identityService.SelfParticipant.Id, cancellationToken);
    }

    async Task IChatFileService.RequestFileDownloadAsync(Guid roomId, FileMetadataMessage fileMetadataMessage, CancellationToken cancellationToken)
    {
        if (fileFeatureCollection.FileStorageService.FileExists(roomId, fileMetadataMessage.FileName)
            || Path.Exists(fileMetadataMessage.FilePath))
        {
            var savedFilePath = fileMetadataMessage.FilePath;
            fileMetadataMessage.FilePath = fileFeatureCollection.FileStorageService
                    .GetFilePath(roomId, fileMetadataMessage.FileName) ?? savedFilePath;

            await eventBus.PublishAsync(new FileTransferCompletedEvent()
            {
                FileName = fileMetadataMessage.FileName,
                FileMetadataId = fileMetadataMessage.Id,
                RoomId = roomId,
                FilePath = fileMetadataMessage.FilePath!,
                TransferId = fileMetadataMessage.TransferId
            }, cancellationToken);
            return;
        }

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

        fileFeatureCollection.FileTransferService.RegisterTransfer(info);

        await eventBus.PublishAsync(new FileTransferStartedEvent
        {
            RoomId = roomId,
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
            FileMetadataId = fileMessage.Id,
        };

        await messageRouter.RouteAsync(message, roomId, identityService.SelfParticipant.Id, cancellationToken);
    }
}
