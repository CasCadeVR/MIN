using MIN.Core.Entities;
using MIN.Core.Events.Contracts;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Rooms;
using MIN.FileTransfer.Events;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.FileTransfer.Services.Contracts.Models;
using MIN.FileTransfer.Services.Contracts.Models.Enums;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.FileTransfer.Handlers;

internal sealed class FileMetadataHandler : IMessageHandler, IFileTransferHandlerAnchor
{
    private readonly IFileTransferService fileTransferService;
    private readonly IEventBus eventBus;
    private readonly IRoomHoster roomHoster;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    public FileMetadataHandler(IFileTransferService fileTransferService,
        IEventBus eventBus,
        IRoomHoster roomHoster,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.fileTransferService = fileTransferService;
        this.eventBus = eventBus;
        this.roomHoster = roomHoster;
        this.identityService = identityService;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.FileMetadata];

    int IMessageHandler.Priority => 5;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not FileMetadataMessage metadata)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(FileMetadataHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(FileMetadataHandler)} - {message.GetType()}");
        }

        logger.Log($"Получены метаданные файла: {metadata.FileName} ({metadata.FileSize} байт) от {metadata.SenderId}");

        if (!context.RoomContext.Participants.TryGetParticipantById(metadata.SenderId, out var sender))
        {
            logger.Log($"Получил метаданные файла от неизвестного отправителя {metadata.SenderId}");
            return HandlerResult.Failure("Получил метаданные файла от неизвестного отправителя", stopPropagation: false);
        }

        context.RoomContext.Messages.AddMessage(metadata);
        var selfId = identityService.SelfParticipant.Id;
        var isSelf = message.SenderId == selfId;

        if (!isSelf)
        {
            logger.Log($"Убираю клиентский путь к файлу для получателя: {metadata.FileName}");
            metadata.FilePath = null;
        }

        if (isSelf || message.RecipientId == selfId || message.IsPublic)
        {
            await eventBus.PublishAsync(new FileMetaDataMessageReceivedEvent()
            {
                RoomId = context.RoomContext.RoomId,
                Message = metadata,
            });
        }

        fileTransferService.RegisterFileMetadata(message.Id, context.RoomContext.RoomId, metadata.FileName, metadata.FilePath);

        var isHosting = roomHoster.IsHosting(context.RoomContext.RoomId);

        if (isHosting && isSelf)
        {
            logger.Log($"Хост: не запрашиваю файл у себя");
            return HandlerResult.Success();
        }

        if (!isHosting || isSelf)
        {
            logger.Log($"Не являюсь хостом или отправитель — не запрашиваю файл: {metadata.FileName}");
            return HandlerResult.Success();
        }

        // Хост получает файл от клиента — запрашиваем загрузку на сервер у клиента
        logger.Log($"Хост: регистрирую загрузку файла {metadata.FileName} (TransferId: {metadata.TransferId})");
        fileTransferService.RegisterPendingMetadata(metadata.TransferId, metadata.FileName);

        var info = new TransferInfo
        {
            TransferId = metadata.TransferId,
            FileMetadataId = metadata.Id,
            RoomId = metadata.RoomId,
            SenderId = selfId,
            Direction = FileTransferDirection.Upload,
            FileName = metadata.FileName,
        };

        fileTransferService.RegisterTransfer(info);

        var requestMessage = new FileTransferRequestMessage
        {
            RoomId = metadata.RoomId,
            TransferId = metadata.TransferId,
            RecipientId = metadata.SenderId,
            FileMetadataId = message.Id,
            Direction = FileTransferDirection.Upload,
        };

        logger.Log($"Отправляю FileTransferRequest (Upload) участнику {metadata.SenderId} для файла {metadata.FileName}");

        await eventBus.PublishAsync(new FileTransferStartedEvent
        {
            RoomId = metadata.RoomId,
            TransferId = metadata.TransferId,
            FileMetadataId = metadata.Id,
            FileName = metadata.FileName,
            FileSize = metadata.FileSize,
            Direction = FileTransferDirection.Upload,
        });

        return HandlerResult.WithResponse(requestMessage);
    }
}
