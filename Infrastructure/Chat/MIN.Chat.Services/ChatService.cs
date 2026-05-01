using MIN.Chat.Messaging;
using MIN.Chat.Services.Contracts.Interfaces;
using MIN.Core.Entities.Contracts.Models;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.FileTransfer.Messaging;

namespace MIN.Chat.Services;

/// <inheritdoc cref="IChatService"/>
public sealed class ChatService : IChatService
{
    private readonly IMessageRouter messageRouter;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatService"/>
    /// </summary>
    public ChatService(IMessageRouter messageRouter)
    {
        this.messageRouter = messageRouter;
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
            FileSize = new FileInfo(filePath).Length,
            RecipientId = recipientId,
        };

        await messageRouter.RouteAsync(message, roomId, sender.Id, cancellationToken);
    }
}
