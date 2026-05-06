using MIN.Chat.Messaging;
using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Events.Contracts;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.FileTransfer.Services.Contracts.Models;
using MIN.FileTransfer.Services.Contracts.Models.Enums;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatService"/>
public sealed class ChatService : IChatService
{
    private readonly IMessageRouter messageRouter;
    private readonly IRoomHoster roomHoster;
    private readonly IEventBus eventBus;
    private readonly IFileHelperService fileHelperService;
    private readonly IFileTransferService fileTransferService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatService"/>
    /// </summary>
    public ChatService(IMessageRouter messageRouter,
        IRoomHoster roomHoster,
        IEventBus eventBus,
        IFileHelperService fileHelperService,
        IFileTransferService fileTransferService)
    {
        this.messageRouter = messageRouter;
        this.roomHoster = roomHoster;
        this.eventBus = eventBus;
        this.fileHelperService = fileHelperService;
        this.fileTransferService = fileTransferService;
    }

    async Task IChatService.SendMessageAsync(Guid roomId, string content, ParticipantInfo sender, Guid? recipientId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Сообщение не должно быть пустым", nameof(content));
        }

        var message = new ChatTextMessage
        {
            RoomId = roomId,
            Sender = sender,
            Content = content,
            RecipientId = recipientId,
        };

        await messageRouter.RouteAsync(message, roomId, sender.Id, cancellationToken);
    }

    async Task IChatService.SendFileAsync(Guid roomId, string fileName, string filePath, ParticipantInfo sender, Guid? recipientId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Файл не найден", nameof(filePath));
        }

        var transferId = Guid.NewGuid();

        var message = new FileMetadataMessage
        {
            TransferId = transferId,
            RoomId = roomId,
            Sender = sender,
            FileName = fileName,
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
                SenderId = sender.Id,
                Direction = FileTransferDirection.Upload,
                FileName = fileName,
            };
            fileTransferService.RegisterTransfer(info);
        }

        await messageRouter.RouteAsync(message, roomId, sender.Id, cancellationToken);
    }

    async Task IChatService.RequestFileDownloadAsync(Guid roomId, FileMetadataMessage fileMessage, ParticipantInfo sender, CancellationToken cancellationToken)
    {
        var transferId = Guid.NewGuid();
        fileMessage.TransferId = transferId;

        var info = new TransferInfo
        {
            TransferId = transferId,
            FileMetadataId = fileMessage.Id,
            RoomId = roomId,
            SenderId = sender.Id,
            Direction = FileTransferDirection.Download,
            FileName = fileMessage.FileName,
        };

        fileTransferService.RegisterTransfer(info);

        await eventBus.PublishAsync(new FileTransferStartedEvent
        {
            RoomId = fileMessage.RoomId,
            TransferId = fileMessage.TransferId,
            FileMetadataId = fileMessage.Id,
            FileName = fileMessage.FileName,
            FileSize = fileMessage.FileSize,
            Direction = FileTransferDirection.Download,
        });

        var message = new FileTransferRequestMessage
        {
            TransferId = transferId,
            RoomId = roomId,
            FileName = fileMessage.FileName,
            FileMetadataId = fileMessage.Id,
            Direction = FileTransferDirection.Download,
        };

        await messageRouter.RouteAsync(message, roomId, sender.Id, cancellationToken);
    }

    async Task IChatService.CancelFileDownloadAsync(Guid roomId, FileMetadataMessage fileMessage, ParticipantInfo sender, CancellationToken cancellationToken)
    {
        var message = new FileTransferCancelMessage
        {
            TransferId = fileMessage.TransferId,
            RoomId = roomId,
            FileMetadataId = fileMessage.Id,
        };

        await messageRouter.RouteAsync(message, roomId, sender.Id, cancellationToken);
    }
}
