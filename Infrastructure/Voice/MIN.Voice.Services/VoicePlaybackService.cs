using System.ComponentModel;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;
using MIN.Helpers.Contracts.Models;
using MIN.Voice.Services.Contacts.Interfaces;
using MIN.Voice.Services.Models;

namespace MIN.Voice.Services;

/// <inheritdoc cref="IVoicePlaybackService"/>
public class VoicePlaybackService : IVoicePlaybackService
{
    private readonly Dictionary<Guid, ParticipantVoiceEntry> channels = [];
    private readonly HashSet<int> voiceCalls = [];
    private readonly IVoiceCodec codec;
    private readonly ISettingsProvider settingsProvider;
    private readonly ILoggerProvider logger;
    private readonly object sync = new();
    private readonly PlaybackDeviceContext? deviceContext;
    private float appVolume;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="VoicePlaybackService"/>
    /// </summary>
    public VoicePlaybackService(IVoiceCodec codec,
        IAudioDeviceService audioDeviceService,
        ISettingsProvider settingsProvider,
        ILoggerProvider logger)
    {
        this.codec = codec;
        this.settingsProvider = settingsProvider;
        this.logger = logger;

        var settings = settingsProvider.GetSettings();
        appVolume = settings.OutputDeviceVolume / 100.0f;

        // Одно устройство/контекст на всё приложение - создаём один раз здесь.
        // Если создать не удалось (нет устройства вывода и т.п.), участники
        // просто не будут создаваться (см. try/catch в AddParticipant),
        // а не будут крашить сервис целиком.
        try
        {
            var outputDevices = audioDeviceService.GetOutputDevices();
            var deviceName = settings.OutputDeviceNumber >= 0 && settings.OutputDeviceNumber < outputDevices.Count
                ? outputDevices[settings.OutputDeviceNumber].Name
                : null;

            deviceContext = new PlaybackDeviceContext(deviceName);
        }
        catch (Exception ex)
        {
            logger.Log($"Не удалось инициализировать устройство воспроизведения: {ex.Message}");
        }

        settingsProvider.OnSettingsSaved += () =>
        {
            lock (sync)
            {
                var currentSettings = settingsProvider.GetSettings();
                appVolume = currentSettings.OutputDeviceVolume / 100.0f;

                var outputDevices = audioDeviceService.GetOutputDevices();
                if (currentSettings.OutputDeviceNumber < 0 || currentSettings.OutputDeviceNumber >= outputDevices.Count)
                {
                    return;
                }

                var device = outputDevices[currentSettings.OutputDeviceNumber];

                try
                {
                    // Меняем устройство ОДИН раз на общем контексте - все
                    // ParticipantChannel пересоздадут свои source через
                    // PlaybackDeviceContext.DeviceChanged.
                    deviceContext?.ChangeDevice(device.Name);
                }
                catch (Exception ex)
                {
                    logger.Log($"Не удалось сменить устройство воспроизведения: {ex.Message}");
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

            if (deviceContext == null)
            {
                logger.Log($"Не удалось создать канал воспроизведения для {participantId}: устройство воспроизведения не инициализировано");
                return;
            }

            var settings = settingsProvider.GetSettings();
            appVolume = settings.OutputDeviceVolume / 100.0f;

            try
            {
                var channel = new ParticipantChannel(codec, deviceContext, appVolume);

                void volumeHandler(object? _, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == nameof(Settings.OutputDeviceVolume))
                    {
                        appVolume = settings.OutputDeviceVolume / 100.0f;
                        channel.ChangeVolume(appVolume, channel.SpecificVolume);
                    }
                }
                settings.PropertyChanged += volumeHandler;

                channels[participantId] = new ParticipantVoiceEntry(channel, volumeHandler);
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
            if (!channels.Remove(participantId, out var entry))
            {
                return;
            }

            settingsProvider.GetSettings().PropertyChanged -= entry.VolumeHandler;
            entry.Channel.Dispose();
        }
    }

    void IVoicePlaybackService.ChangeParticipantVolume(Guid participantId, int specificVolume)
    {
        ParticipantChannel? channel;
        lock (sync)
        {
            channels.TryGetValue(participantId, out var entry);
            channel = entry?.Channel;
        }

        channel?.ChangeVolume(appVolume, specificVolume / 100.0f);
    }

    void IVoicePlaybackService.PlaySamples(Guid participantId, long sequenceNumber, byte[] data)
    {
        try
        {
            ParticipantChannel? channel;
            lock (sync)
            {
                channels.TryGetValue(participantId, out var entry);
                channel = entry?.Channel;
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
            var settings = settingsProvider.GetSettings();

            foreach (var entry in channels.Values)
            {
                settings.PropertyChanged -= entry.VolumeHandler;
                entry.Channel.Dispose();
            }

            voiceCalls.Clear();
            channels.Clear();
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        Clear();
        deviceContext?.Dispose();
    }
}
