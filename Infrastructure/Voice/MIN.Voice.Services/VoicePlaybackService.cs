using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;
using MIN.Helpers.Contracts.Models;
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
    private int appVolume;

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
            lock (sync)
            {
                appVolume = settingsProvider.GetSettings().OutputDeviceVolume;

                var avaibleDevices = WaveInterop.waveOutGetNumDevs() - 1;

                if (avaibleDevices <= 0)
                {
                    return;
                }

                var clamped = Math.Clamp(settingsProvider.GetSettings().OutputDeviceNumber, 0, avaibleDevices);

                foreach (var channel in channels)
                {
                    channel.Value.ChangeDevice(clamped);
                }
            }
        };
    }

    /// <inheritdoc />
    public void RegisterSubroomVoice(int subRoomId)
        => voiceCalls.Add(subRoomId);

    /// <inheritdoc />
    public void UnregisterSubroomVoice(int subRoomId)
        => voiceCalls.Remove(subRoomId);

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

            appVolume = settings.OutputDeviceVolume;

            try
            {
                var channel = new ParticipantChannel(codec, deviceNumber, appVolume);

                settings.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(Settings.OutputDeviceVolume))
                    {
                        appVolume = settings.OutputDeviceVolume;
                        channel.ChangeVolume(appVolume, channel.SpecificVolume);
                    }
                };

                channels[participantId] = channel;
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

    void IVoicePlaybackService.ChangeParticipantVolume(Guid participantId, int specificVolume)
    {
        lock (sync)
        {
            ParticipantChannel? channel;
            lock (sync)
            {
                channels.TryGetValue(participantId, out channel);
            }

            channel?.ChangeVolume(appVolume, specificVolume);
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
