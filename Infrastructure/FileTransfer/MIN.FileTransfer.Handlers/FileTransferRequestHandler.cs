using MIN.Core.Handlers.Contracts;
using MIN.Core.Handlers.Contracts.Models;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Core.Streaming.Contracts.Interfaces;
using MIN.Core.Streaming.Contracts.Models;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services.Contracts.Interfaces;
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

            logger.Log($"Начинаю загрузку файла {info.FileName} на сервер (TransferId: {request.TransferId})");

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
                logger.Log($"Регистрирую новый download transfer для файла {request.FileName} (TransferId: {request.TransferId})");
                fileTransferService.RegisterTransfer(request.TransferId, request.RoomId, FileTransferDirection.Download, request.FileName);
            }
            else
            {
                logger.Log($"Transfer {request.TransferId} уже зарегистрирован, начинаю download");
            }

            var response = new FileTransferResponseMessage
            {
                RoomId = request.RoomId,
                TransferId = request.TransferId,
                Success = true,
            };

            fileTransferService.TryGetTransferInfo(request.TransferId, out var transferInfo);

            logger.Log($"Отправляю FileTransferResponse (Success) для TransferId={request.TransferId}");
            await messageSender.SendAsync(response, request.RoomId, context.ConnectionId, context.CancellationToken);

            var filePath = fileStorageService.GetFilePath(transferInfo.RoomId, transferInfo.FileName);
            if (filePath == null)
            {
                logger.Log($"Файл не найден на сервере: {transferInfo.FileName} (TransferId: {response.TransferId})");

                var errorResponse = new FileTransferResponseMessage
                {
                    RoomId = transferInfo.RoomId,
                    TransferId = response.TransferId,
                    Success = false,
                    ErrorMessage = "File not found on server",
                };

                await messageSender.SendAsync(errorResponse, transferInfo.RoomId, context.ConnectionId, context.CancellationToken);

                return HandlerResult.Failure("File not found on server", stopPropagation: false);
            }

            logger.Log($"Начинаю стриминг файла {transferInfo.FileName} ({new FileInfo(filePath).Length} байт)");

            await using var fileStream = File.OpenRead(filePath);
            var fileBytes = new byte[fileStream.Length];
            await fileStream.ReadAsync(fileBytes, context.CancellationToken);

            var options = new StreamOptions
            {
                RequiresAcks = true,
                RequiresEncryption = true,
            };

            logger.Log($"Отправляю файл через StreamManager: TransferId={response.TransferId}");
            await streamManager.SendAsync(fileBytes, options, transferInfo.RoomId, context.ConnectionId, context.CancellationToken);

            return HandlerResult.Success(stopPropagation: true);
        }
    }
}
