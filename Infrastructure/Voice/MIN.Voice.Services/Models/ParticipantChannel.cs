using MIN.Voice.Services.Contacts.Constants;
using MIN.Voice.Services.Contacts.Interfaces;
using NAudio.Wave;

namespace MIN.Voice.Services.Models;

/// <summary>
/// Канал звука для одного участника
/// </summary>
internal sealed class ParticipantChannel : IDisposable
{
    private const int MaxBufferedFrames = 10;
    private readonly IVoiceCodec codec;
    private readonly BufferedWaveProvider provider;
    private readonly SortedDictionary<long, byte[]> pending = [];
    private readonly object gate = new();
    private int deviceNumber;
    private WaveOutEvent waveOut = null!;
    private long? expectedNextSequence;

    /// <summary>
    /// Специфичный для участника громкость
    /// </summary>
    public int SpecificVolume;

    public ParticipantChannel(IVoiceCodec codec, int deviceNumber, int startVolume, int specificVolume = 100)
    {
        this.codec = codec;

        var waveFormat = new WaveFormat(
            VoiceAudioConstants.SampleRate,
            VoiceAudioConstants.BitsPerSample,
            VoiceAudioConstants.Channels);

        provider = new BufferedWaveProvider(waveFormat)
        {
            BufferDuration = TimeSpan.FromMilliseconds(500),
            DiscardOnBufferOverflow = true,
        };

        this.deviceNumber = deviceNumber;
        SpecificVolume = specificVolume;

        InitNewWave(startVolume, specificVolume);
    }

    private void InitNewWave(int startVolume, int specificVolume)
    {
        waveOut = new WaveOutEvent
        {
            Volume = ConvertVolumes(startVolume, specificVolume),
            DeviceNumber = deviceNumber,
            DesiredLatency = 100,
        };
        waveOut.Init(provider);
        waveOut.Play();
    }

    /// <summary>
    /// Поменять звук потока
    /// </summary>
    public void ChangeVolume(int appVolume, int specificVolume)
    {
        SpecificVolume = specificVolume;
        waveOut?.Volume = ConvertVolumes(appVolume, specificVolume);
    }

    private static float ConvertVolumes(int appVolume, int specificVolume)
        => Math.Clamp((appVolume / 100.0f) * (specificVolume / 100.0f), 0f, 1f);

    /// <summary>
    /// Обновить устройство вывода
    /// </summary>
    public void ChangeDevice(int deviceNumber)
    {
        if (this.deviceNumber == deviceNumber)
        {
            return;
        }

        this.deviceNumber = deviceNumber;
        var savedVolume = waveOut.Volume;

        waveOut.Stop();
        waveOut.Dispose();

        InitNewWave((int)savedVolume, SpecificVolume);
    }

    public void Enqueue(long sequenceNumber, byte[] data)
    {
        var decoded = codec.Decode(data);

        lock (gate)
        {
            expectedNextSequence ??= sequenceNumber;

            if (sequenceNumber < expectedNextSequence.Value)
            {
                return;
            }

            pending[sequenceNumber] = decoded;

            if (pending.Count > MaxBufferedFrames)
            {
                var oldest = pending.Keys.First();
                expectedNextSequence = Math.Max(expectedNextSequence.Value, oldest + 1);
                pending.Remove(oldest);
            }

            FlushReady();
        }
    }

    private void FlushReady()
    {
        while (pending.TryGetValue(expectedNextSequence!.Value, out var samples))
        {
            provider.AddSamples(samples, 0, samples.Length);
            pending.Remove(expectedNextSequence.Value);
            expectedNextSequence = expectedNextSequence.Value + 1;
        }
    }

    public void Dispose()
    {
        waveOut.Stop();
        waveOut.Dispose();

        lock (gate)
        {
            pending.Clear();
        }
    }
}
