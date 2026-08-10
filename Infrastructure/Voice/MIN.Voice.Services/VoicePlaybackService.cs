using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;
using MIN.Voice.Services.Contacts.Interfaces;
using MIN.Voice.Services.Models;
using NAudio.Wave;

namespace MIN.Voice.Services;

/// <inheritdoc cref="IVoicePlaybackService"/>
public class VoicePlaybackService : IVoicePlaybackService
{
    private readonly Dictionary<Guid, ParticipantChannel> channels = [];
    private readonly HashSet<int> voiceCalls = [];
    private readonly IVoiceCodec codec;
    private readonly ISettingsProvider settingsProvider;
    private readonly ILoggerProvider logger;
    private readonly object sync = new();

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="VoicePlaybackService"/>
    /// </summary>
    public VoicePlaybackService(IVoiceCodec codec, ISettingsProvider settingsProvider, ILoggerProvider logger)
    {
        this.codec = codec;
        this.settingsProvider = settingsProvider;
        this.logger = logger;

        settingsProvider.OnSettingsSaved += () =>
        {
            // Restart audio playback cuz dynamics might have changed

            lock (sync)
            {
                var savedSubrooms = voiceCalls.ToList();
                var savedGuids = channels.Keys.ToList();

                Clear();

                foreach (var subRoomId in savedSubrooms)
                {
                    RegisterSubroomVoice(subRoomId);
                }

                foreach (var participantId in savedGuids)
                {
                    AddParticipant(participantId);
                }
            }
        };
    }

    /// <inheritdoc />
    public void RegisterSubroomVoice(int subRoomId)
        => voiceCalls.Add(subRoomId);

    bool IVoicePlaybackService.IsInVoiceCall(int subRoomId)
        => voiceCalls.Contains(subRoomId);

    /// <inheritdoc />
    public void AddParticipant(Guid participantId)
    {
        lock (sync)
        {
            if (channels.ContainsKey(participantId))
            {
                return;
            }

            var settings = settingsProvider.GetSettings();

            var deviceNumber = settings.OutputDeviceNumber < 0
                ? -1
                : Math.Min(settings.OutputDeviceNumber, WaveInterop.waveOutGetNumDevs() - 1);

            try
            {
                channels[participantId] = new ParticipantChannel(codec, deviceNumber);
            }
            catch (Exception ex)
            {
                logger.Log($"Не удалось создать канал воспроизведения для {participantId}: {ex.Message}");
            }
        }
    }

    void IVoicePlaybackService.RemoveParticipant(Guid participantId)
    {
        lock (sync)
        {
            if (!channels.Remove(participantId, out var channel))
            {
                return;
            }

            channel.Dispose();
        }
    }

    void IVoicePlaybackService.PlaySamples(Guid participantId, long sequenceNumber, byte[] data)
    {
        try
        {
            ParticipantChannel? channel;
            lock (sync)
            {
                channels.TryGetValue(participantId, out channel);
            }

            channel?.Enqueue(sequenceNumber, data);
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка воспроизведения голоса для {participantId}: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        lock (sync)
        {
            foreach (var channel in channels.Values)
            {
                channel.Dispose();
            }

            voiceCalls.Clear();

            channels.Clear();
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => ((IVoicePlaybackService)this).Clear();
}
