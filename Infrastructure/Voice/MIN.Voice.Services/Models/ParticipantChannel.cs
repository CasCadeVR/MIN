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
    private readonly WaveOutEvent waveOut;
    private readonly SortedDictionary<long, byte[]> pending = [];
    private readonly object gate = new();
    private long? expectedNextSequence;

    public ParticipantChannel(IVoiceCodec codec, int deviceNumber)
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

        waveOut = new WaveOutEvent
        {
            DeviceNumber = deviceNumber,
            DesiredLatency = 100,
        };
        waveOut.Init(provider);
        waveOut.Play();
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
