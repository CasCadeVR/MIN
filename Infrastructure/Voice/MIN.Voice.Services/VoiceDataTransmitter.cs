using System.Threading.Channels;
using MIN.Core.Identity.Contracts.Interfaces;
using MIN.Core.Services.Contracts.Interfaces.Messaging;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Models.Enums;
using MIN.Voice.Messaging;
using MIN.Voice.Services.Contacts.Interfaces;
using MIN.Voice.Services.Contacts.Models;

namespace MIN.Voice.Services;

/// <inheritdoc cref="IVoiceDataTransmitter"/>
public class VoiceDataTransmitter : IVoiceDataTransmitter
{
    private readonly IAudioCaptureService audioCaptureService;
    private readonly IMessageRouter messageRouter;
    private readonly IVoiceCodec codec;
    private readonly IIdentityService identityService;
    private readonly ILoggerProvider logger;

    private Channel<VoiceDataMessage> queue = null!;
    private CancellationTokenSource? sendCts;
    private Task? sendTask;

    private Guid roomId;
    private int subRoomId;
    private long sequenceNumber;
    private bool isActive;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="VoiceDataTransmitter"/>
    /// </summary>
    public VoiceDataTransmitter(IAudioCaptureService audioCaptureService,
        IMessageRouter messageRouter,
        IVoiceCodec codec,
        IIdentityService identityService,
        ILoggerProvider logger)
    {
        this.audioCaptureService = audioCaptureService;
        this.messageRouter = messageRouter;
        this.codec = codec;
        this.identityService = identityService;
        this.logger = logger;
    }

    /// <inheritdoc />
    public void Begin(Guid roomId, int subRoomId)
    {
        if (isActive)
        {
            return;
        }

        this.roomId = roomId;
        this.subRoomId = subRoomId;
        isActive = true;

        queue = Channel.CreateBounded<VoiceDataMessage>(new BoundedChannelOptions(64)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
        });

        sendCts = new CancellationTokenSource();
        sendTask = SendPumpAsync(sendCts.Token);

        audioCaptureService.FrameCaptured += OnFrameCaptured;
    }

    /// <inheritdoc />
    public void End()
    {
        if (!isActive)
        {
            return;
        }

        isActive = false;

        audioCaptureService.FrameCaptured -= OnFrameCaptured;

        queue.Writer.TryComplete();
        sendCts?.Cancel();
        sendCts = null;
        sendTask = null;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => End();

    private void OnFrameCaptured(object? sender, AudioFrame e)
    {
        if (!isActive)
        {
            return;
        }

        var currentSequence = Interlocked.Increment(ref sequenceNumber);

        var message = new VoiceDataMessage
        {
            SubRoomId = subRoomId,
            SequenceNumber = currentSequence,
            Codec = codec.Kind,
            Data = codec.Encode(e.Data),
        };

        queue.Writer.TryWrite(message);
    }

    private async Task SendPumpAsync(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var message in queue.Reader.ReadAllAsync(cancellationToken))
            {
                try
                {
                    await messageRouter.RouteAsync(message, roomId, identityService.SelfParticipant.Id, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    logger.Log($"Ошибка отправки голосового кадра: {ex.Message}", LogLevel.Error);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            queue.Writer.TryComplete();
        }
    }
}
