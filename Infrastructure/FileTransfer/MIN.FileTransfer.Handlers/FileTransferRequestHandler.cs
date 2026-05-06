using System.Reflection;
using MIN.Core.Entities;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.Core.Streaming.Contracts.Models;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
using MIN.FileTransfer.Services.Contracts.Models;
using MIN.FileTransfer.Services.Contracts.Models.Enums;
using MIN.Helpers.Contracts.Interfaces;

namespace MIN.FileTransfer.Handlers;

internal sealed class FileTransferRequestHandler : IMessageHandler, IFileTransferHandlerAnchor
{
    private readonly IIdentityService identityService;
    private readonly IFileTransferService fileTransferService;
    private readonly IFileStorageService fileStorageService;
    private readonly IStreamManager streamManager;
    private readonly IMessageSender messageSender;
    private readonly ILoggerProvider logger;

    public FileTransferRequestHandler(
        IIdentityService identityService,
        IFileTransferService fileTransferService,
        IFileStorageService fileStorageService,
        IStreamManager streamManager,
        IMessageSender messageSender,
        ILoggerProvider logger)
    {
        this.identityService = identityService;
        this.fileTransferService = fileTransferService;
        this.fileStorageService = fileStorageService;
        this.streamManager = streamManager;
        this.messageSender = messageSender;
        this.logger = logger;
    }

    IEnumerable<MessageTypeTag> IMessageHandler.HandledTypes => [MessageTypeTag.FileTransferRequest];

    int IMessageHandler.Priority => 5;

    async Task<HandlerResult> IMessageHandler.HandleAsync(IMessage message, MessageContext context)
    {
        if (message is not FileTransferRequestMessage request)
        {
            logger.Log($"Неизвестный тип сообщения в {nameof(FileTransferRequestHandler)} - {message.GetType()}");
            return HandlerResult.Failure($"Неизвестный тип сообщения в {nameof(FileTransferRequestHandler)} - {message.GetType()}");
        }

        logger.Log($"Получен FileTransferRequest: TransferId={request.TransferId}, Direction={request.Direction}, FileName={request.FileName}");

        var selfId = identityService.SelfParticipant.Id;

        if (request.Direction == FileTransferDirection.Upload)
        {
            return await HandleUpload(request, selfId, context);
        }
        else
        {
            return await HandleDownload(request, selfId, context);
        }
    }

    private async Task<HandlerResult> HandleUpload(FileTransferRequestMessage request, Guid selfId, MessageContext context)
    {
        if (request.RecipientId != selfId)
        {
            logger.Log($"Запрос на загрузку файла адресован не мне (мне: {selfId}, запрос: {request.RecipientId})");
            return HandlerResult.Failure("Запрос на загрузку файла адресован не мне", stopPropagation: false);
        }

        if (!fileTransferService.TryGetTransferInfo(request.TransferId, out var info))
        {
            logger.Log($"Не найдена информация о transfer {request.TransferId}");
            return HandlerResult.Failure($"Не найдена информация о transfer {request.TransferId}", stopPropagation: false);
        }

        var filePath = ResolveFilePath(request.FileMetadataId, info.RoomId, info.FileName);
        if (filePath == null)
        {
            logger.Log($"Файл не найден для upload: {info.FileName} (TransferId: {request.TransferId})");
            var errorResponse = new FileTransferResponseMessage
            {
                RoomId = request.RoomId,
                TransferId = request.TransferId,
                Success = false,
                ErrorMessage = "File not found",
            };

            await messageSender.SendAsync(errorResponse, request.RoomId, context.ConnectionId, context.CancellationToken);
            return HandlerResult.Failure("File not found", stopPropagation: true);
        }

        logger.Log($"Начинаю загрузку файла {info.FileName} на сервер из: {filePath}");

        var response = new FileTransferResponseMessage
        {
            RoomId = request.RoomId,
            TransferId = request.TransferId,
            Success = true,
        };

        await messageSender.SendAsync(response, request.RoomId, context.ConnectionId, context.CancellationToken);

        await StreamFileAsync(filePath, request, context);

        return HandlerResult.Success(stopPropagation: true);
    }

    private async Task<HandlerResult> HandleDownload(FileTransferRequestMessage request, Guid selfId, MessageContext context)
    {
        if (!fileTransferService.TryGetTransferInfo(request.TransferId, out _))
        {
            logger.Log($"Регистрирую новый download transfer для файла {request.FileName} (TransferId: {request.TransferId})");

            var info = new TransferInfo
            {
                TransferId = request.TransferId,
                FileMetadataId = request.FileMetadataId,
                RoomId = request.RoomId,
                SenderId = selfId,
                Direction = FileTransferDirection.Download,
                FileName = request.FileName,
            };

            fileTransferService.RegisterTransfer(info);
        }
        else
        {
            logger.Log($"Transfer {request.TransferId} уже зарегистрирован, начинаю download");
        }

        var filePath = ResolveFilePath(request.FileMetadataId, request.RoomId, request.FileName);
        if (filePath == null)
        {
            logger.Log($"Файл не найден для download: {request.FileName} (TransferId: {request.TransferId})");
            var errorResponse = new FileTransferResponseMessage
            {
                RoomId = request.RoomId,
                TransferId = request.TransferId,
                Success = false,
                ErrorMessage = "File not found on server",
            };

            await messageSender.SendAsync(errorResponse, request.RoomId, context.ConnectionId, context.CancellationToken);
            return HandlerResult.Failure("File not found on server", stopPropagation: false);
        }

        logger.Log($"Начинаю download файла {request.FileName} из: {filePath}");

        var response = new FileTransferResponseMessage
        {
            RoomId = request.RoomId,
            TransferId = request.TransferId,
            Success = true,
        };

        await messageSender.SendAsync(response, request.RoomId, context.ConnectionId, context.CancellationToken);

        await StreamFileAsync(filePath, request, context);

        return HandlerResult.Success(stopPropagation: true);
    }

    private string? ResolveFilePath(Guid fileMetadataId, Guid roomId, string fileName)
    {
        fileTransferService.TryGetFileMetadata(fileMetadataId, out var fileMetadataInfo);

        if (fileMetadataInfo.IsStoredOnServer)
        {
            var filePath = fileStorageService.GetFilePath(roomId, fileName);
            if (filePath != null)
            {
                logger.Log($"Файл найден в хранилище: {filePath}");
                return filePath;
            }
        }

        var originalPath = fileMetadataInfo.OriginalPath;

        if (originalPath != null && File.Exists(originalPath))
        {
            logger.Log($"Файл найден по оригинальному пути: {originalPath}");
            return originalPath;
        }

        return null;
    }

    private async Task StreamFileAsync(string filePath, FileTransferRequestMessage request, MessageContext context)
    {
        if (!fileTransferService.TryGetTransferInfo(request.TransferId, out var info))
        {
            logger.Log($"Не найдена информация о передаче {request.TransferId} во время стриминга файла");
            return;
        }

        info.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);

        var fileSize = new FileInfo(filePath).Length;
        logger.Log($"Начинаю стриминг файла {Path.GetFileName(filePath)} ({fileSize} байт)");

        var options = new StreamOptions
        {
            RequiresAcks = true,
            RequiresEncryption = true,
            StreamId = request.TransferId,
            IsRawPayload = true,
        };

        logger.Log($"Отправляю файл через StreamManager: StreamId={request.TransferId}");

        await using var fileStream = File.OpenRead(filePath);
        await streamManager.SendAsync(fileStream, options, request.RoomId, context.ConnectionId, info.CancellationTokenSource.Token);
    }
}
