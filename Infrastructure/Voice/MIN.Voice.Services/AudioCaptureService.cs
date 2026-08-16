using System.Threading.Channels;
using MIN.Helpers.Contracts.Interfaces;
using MIN.Helpers.Contracts.Interfaces.SettingsServices;
using MIN.Helpers.Contracts.Models;
using MIN.Voice.Services.Contacts.Constants;
using MIN.Voice.Services.Contacts.Interfaces;
using MIN.Voice.Services.Contacts.Models;
using OpenTK.Audio.OpenAL;

namespace MIN.Voice.Services;

/// <inheritdoc cref="IAudioCaptureService"/>
public class AudioCaptureService : IAudioCaptureService
{
    private const int BufferFrameMultiplier = 4;
    private const int IdleSleepMs = 5;

    private readonly ILoggerProvider logger;
    private readonly ISettingsProvider settingsProvider;
    private readonly SemaphoreSlim controlLock = new(1, 1); // async-safe, no deadlock on Join
    private readonly int frameSampleCount;

    private CancellationTokenSource? cts;
    private string? currentDeviceName;
    private Thread? captureThread;
    private Thread? dispatchThread;
    private Channel<AudioFrame>? frameChannel;
    private bool isStarted;

    // volatile: written from the settings-saved callback (arbitrary thread),
    // read once per frame on the capture thread. A float fits in a machine
    // word so the read/write is already atomic - volatile just guarantees the
    // capture thread doesn't cache a stale value across loop iterations.
    private volatile float micGain = 1f;

    /// <inheritdoc />
    public event EventHandler<AudioFrame>? FrameCaptured;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="AudioCaptureService"/>
    /// </summary>
    public AudioCaptureService(ILoggerProvider logger, ISettingsProvider settingsProvider)
    {
        this.logger = logger;
        this.settingsProvider = settingsProvider;

        frameSampleCount = VoiceAudioConstants.SampleRate * VoiceAudioConstants.FrameDurationMs / 1000;

        micGain = ReadMicGain();

        settingsProvider.OnSettingsSaved += async () =>
        {
            await controlLock.WaitAsync();
            try
            {
                if (!isStarted)
                {
                    return;
                }

                var settings = settingsProvider.GetSettings();
                settings.PropertyChanged -= Settings_PropertyChanged;
                settings.PropertyChanged += Settings_PropertyChanged;

                var newDeviceName = GetDeviceNameByIndex(settings.InputDeviceNumber);

                if (currentDeviceName == newDeviceName)
                {
                    return;
                }

                await StopInternalAsync();
                StartInternal();
            }
            finally
            {
                controlLock.Release();
            }
        };
    }

    /// <inheritdoc />
    public void Start()
    {
        controlLock.Wait();
        try
        {
            if (isStarted)
            {
                return;
            }

            StartInternal();
        }
        finally
        {
            controlLock.Release();
        }
    }

    private void StartInternal()
    {
        var settings = settingsProvider.GetSettings();
        settings.PropertyChanged += Settings_PropertyChanged;

        var deviceName = GetDeviceNameByIndex(settings.InputDeviceNumber);

        var format = VoiceAudioConstants.Channels == 1
            ? ALFormat.Mono16
            : ALFormat.Stereo16;

        var bufferSize = frameSampleCount * BufferFrameMultiplier;

        var device = ALC.CaptureOpenDevice(deviceName, VoiceAudioConstants.SampleRate, format, bufferSize);

        if (device == ALCaptureDevice.Null)
        {
            logger.Log($"Не удалось открыть устройство захвата '{deviceName ?? "по умолчанию"}'");
            return;
        }

        currentDeviceName = deviceName;

        ALC.CaptureStart(device);

        isStarted = true;
        cts = new CancellationTokenSource();
        var token = cts.Token;

        // Bounded channel = natural backpressure instead of unbounded ThreadPool fan-out.
        // Single reader guarantees frames are dispatched in the exact order they were captured.
        frameChannel = Channel.CreateBounded<AudioFrame>(new BoundedChannelOptions(64)
        {
            SingleReader = true,
            SingleWriter = true,
            FullMode = BoundedChannelFullMode.DropOldest // if consumer falls behind, drop stale audio, not fresh
        });

        captureThread = new Thread(() => CaptureLoop(device, token))
        {
            Name = "AudioCaptureThread",
            IsBackground = true
        };
        captureThread.Start();

        dispatchThread = new Thread(() => DispatchLoop(frameChannel.Reader, token))
        {
            Name = "AudioDispatchThread",
            IsBackground = true
        };
        dispatchThread.Start();

        logger.Log($"Захват запущен на устройстве '{deviceName ?? "микрофона, выбранному по умолчанию"}'");
    }

    private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.InputDeviceVolume))
        {
            micGain = ReadMicGain();
        }
    }

    private float ReadMicGain()
    {
        var settings = settingsProvider.GetSettings();
        var percent = settings.InputDeviceVolume / 100f;
        return Math.Clamp(percent, 0f, 1.0f);
    }

    // Producer: ONLY talks to OpenAL and writes to the channel. No locking, no event invocation here.
    private void CaptureLoop(ALCaptureDevice device, CancellationToken token)
    {
        var frameBuffer = new byte[frameSampleCount * 2];
        var samples = new short[frameSampleCount];
        var writer = frameChannel!.Writer;

        while (!token.IsCancellationRequested)
        {
            try
            {
                var samplesAvailable = ALC.GetInteger(device, AlcGetInteger.CaptureSamples);

                if (samplesAvailable >= frameSampleCount)
                {
                    ALC.CaptureSamples(device, samples, frameSampleCount);

                    ApplyGain(samples, micGain);

                    Buffer.BlockCopy(samples, 0, frameBuffer, 0, frameBuffer.Length);

                    var frameCopy = new byte[frameBuffer.Length];
                    Buffer.BlockCopy(frameBuffer, 0, frameCopy, 0, frameBuffer.Length);

                    // TryWrite never blocks the capture loop; DropOldest keeps latency bounded
                    writer.TryWrite(new AudioFrame(frameCopy));
                }
                else
                {
                    Thread.Sleep(IdleSleepMs);
                }
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка в цикле захвата: {ex.Message}");
                if (ex is ObjectDisposedException or InvalidOperationException)
                {
                    break;
                }
                Thread.Sleep(IdleSleepMs * 10);
            }
        }

        writer.TryComplete();

        try
        {
            ALC.CaptureStop(device);
            ALC.CaptureCloseDevice(device);
        }
        catch (Exception ex)
        {
            logger.Log($"Ошибка при остановке устройства захвата: {ex.Message}");
        }

        logger.Log("Цикл захвата завершён");
    }

    // Scales 16-bit PCM samples in place. gain == 1f is a no-op fast path so
    // the common case (mic at 100%) costs nothing extra per frame.
    private static void ApplyGain(short[] samples, float gain)
    {
        if (gain == 1f)
        {
            return;
        }

        if (gain == 0f)
        {
            Array.Clear(samples, 0, samples.Length);
            return;
        }

        for (var i = 0; i < samples.Length; i++)
        {
            var scaled = samples[i] * gain;
            samples[i] = (short)Math.Clamp(scaled, short.MinValue, short.MaxValue);
        }
    }

    // Consumer: single thread reading the channel = strict FIFO delivery to subscribers, no reordering.
    private void DispatchLoop(ChannelReader<AudioFrame> reader, CancellationToken token)
    {
        try
        {
            foreach (var frame in reader.ReadAllAsync(token).ToBlockingEnumerable(token))
            {
                try
                {
                    FrameCaptured?.Invoke(this, frame);
                }
                catch (Exception ex)
                {
                    logger.Log($"Ошибка в обработчике FrameCaptured: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // expected on stop
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        controlLock.Wait();
        try
        {
            if (!isStarted)
            {
                return;
            }

            StopInternalAsync().GetAwaiter().GetResult();
        }
        finally
        {
            controlLock.Release();
        }
    }

    // No lock is held while waiting for the threads to exit, so they're free to reach
    // their own cleanup code (which no longer needs to touch shared state under lock at all).
    private Task StopInternalAsync()
    {
        cts?.Cancel();

        return Task.Run(() =>
        {
            captureThread?.Join();
            dispatchThread?.Join();

            currentDeviceName = null;
            settingsProvider.GetSettings().PropertyChanged -= Settings_PropertyChanged;
            isStarted = false;

            cts?.Dispose();
            cts = null;

            logger.Log("Захват остановлен");
        });
    }

    private static string? GetDeviceNameByIndex(int index)
    {
        var devices = GetCaptureDevices();
        return index >= 0 && index < devices.Count ? devices[index] : null;
    }

    private static List<string> GetCaptureDevices()
        => ALC.GetString(ALDevice.Null, AlcGetStringList.CaptureDeviceSpecifier);

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => Stop();
}
