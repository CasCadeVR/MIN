using MIN.Chat.Messaging;
using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Models.Enums;
using MIN.FileTransfer.Services.Contracts.Interfaces;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatService"/>
public sealed class ChatService : IChatService
{
    private readonly IMessageRouter messageRouter;
    private readonly IFileHelperService fileHelperService;
    private readonly IFileTransferService fileTransferService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatService"/>
    /// </summary>
    public ChatService(IMessageRouter messageRouter,
        IFileHelperService fileHelperService,
        IFileTransferService fileTransferService)
    {
        this.messageRouter = messageRouter;
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

        var message = new FileMetadataMessage
        {
            TransferId = Guid.NewGuid(),
            RoomId = roomId,
            Sender = sender,
            FileName = fileName,
            FilePath = filePath,
            FileSize = fileHelperService.GetFileSize(filePath),
            RecipientId = recipientId,
        };

        await messageRouter.RouteAsync(message, roomId, sender.Id, cancellationToken);
    }

    async Task IChatService.RequestFileDownloadAsync(Guid roomId, FileMetadataMessage fileMessage, ParticipantInfo sender, CancellationToken cancellationToken)
    {
        var transferId = Guid.NewGuid();
        fileTransferService.RegisterTransfer(transferId, roomId, FileTransferDirection.Download, fileMessage.FileName);

        var message = new FileTransferRequestMessage
        {
            TransferId = transferId,
            RoomId = roomId,
            FileName = fileMessage.FileName,
            Direction = FileTransferDirection.Download,
        };

        await messageRouter.RouteAsync(message, roomId, sender.Id, cancellationToken);
    }
}
