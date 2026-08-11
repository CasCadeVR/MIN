using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;
using MIN.Voice.Services.Contacts.Constants;
using MIN.Voice.Services.Contacts.Interfaces;
using MIN.Voice.Services.Contacts.Models;
using NAudio.Wave;

namespace MIN.Voice.Services;

/// <inheritdoc cref="IAudioCaptureService"/>
public class AudioCaptureService : IAudioCaptureService
{
    private readonly ILoggerProvider logger;
    private readonly ISettingsProvider settingsProvider;
    private WaveInEvent? waveIn;
    private bool isStarted;

    /// <inheritdoc />
    public event EventHandler<AudioFrame>? FrameCaptured;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="AudioCaptureService"/>
    /// </summary>
    public AudioCaptureService(ILoggerProvider logger, ISettingsProvider settingsProvider)
    {
        this.logger = logger;
        this.settingsProvider = settingsProvider;

        settingsProvider.OnSettingsSaved += () =>
        {
            var deviceNumber = Math.Clamp(settingsProvider.GetSettings().InputDeviceNumber, 0, WaveInEvent.DeviceCount - 1);
            waveIn?.DeviceNumber = deviceNumber;
        };
    }

    /// <inheritdoc />
    public void Start()
    {
        if (isStarted)
        {
            return;
        }

        if (WaveInEvent.DeviceCount == 0)
        {
            logger.Log("Не найдено устройство записи, захват звука невозможен");
            return;
        }

        var waveFormat = new WaveFormat(
            VoiceAudioConstants.SampleRate,
            VoiceAudioConstants.BitsPerSample,
            VoiceAudioConstants.Channels);

        var deviceNumber = Math.Clamp(settingsProvider.GetSettings().InputDeviceNumber, 0, WaveInEvent.DeviceCount - 1);

        waveIn = new WaveInEvent
        {
            DeviceNumber = deviceNumber,
            WaveFormat = waveFormat,
            BufferMilliseconds = VoiceAudioConstants.FrameDurationMs,
        };

        waveIn.DataAvailable += OnDataAvailable;
        waveIn.RecordingStopped += OnRecordingStopped;

        isStarted = true;
        waveIn.StartRecording();
    }

    /// <inheritdoc />
    public void Stop()
    {
        if (!isStarted)
        {
            return;
        }

        isStarted = false;

        var current = waveIn;
        waveIn = null;

        if (current == null)
        {
            return;
        }

        current.DataAvailable -= OnDataAvailable;
        current.RecordingStopped -= OnRecordingStopped;

        try
        {
            current.StopRecording();
        }
        catch (Exception ex)
        {
            logger.Log($"Не удалось остановить захват звука: {ex.Message}");
        }
        finally
        {
            current.Dispose();
        }
    }

    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        try
        {
            var buffer = new byte[e.BytesRecorded];
            Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesRecorded);

            FrameCaptured?.Invoke(this, new AudioFrame(buffer));
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка во время захвата звука: {ex.Message}");
        }
    }

    private void OnRecordingStopped(object? sender, StoppedEventArgs e)
    {
        isStarted = false;
        waveIn = null;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => Stop();
}
