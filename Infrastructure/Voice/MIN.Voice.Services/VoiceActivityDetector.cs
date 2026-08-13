using MIN.Helpers.Contracts.Interfaces.SettingsServices;
using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.Services;

/// <inheritdoc cref="IVoiceActivityDetector"/>
public class VoiceActivityDetector : IVoiceActivityDetector
{
    private readonly ISettingsProvider settingsProvider;

    private DateTime lastSpeechTime = DateTime.MinValue;
    private TimeSpan holdTime = TimeSpan.FromMilliseconds(400);

    private readonly Queue<float> recentRms = new(100);
    private float noiseFloor = -60f;
    private bool noiseFloorInitialized;
    private readonly object @lock = new();

    private int sensitivityDb;
    private int holdTimeMs;
    private int adaptiveMarginDb;
    private float zcrThreshold;
    private int zcrSpeechRmsThreshold;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="VoiceActivityDetector"/>
    /// </summary>
    public VoiceActivityDetector(ISettingsProvider settingsProvider)
    {
        this.settingsProvider = settingsProvider;
        this.settingsProvider.OnSettingsSaved += OnSettingsChanged;
        LoadSettings();
    }

    private void LoadSettings()
    {
        var settings = settingsProvider.GetSettings();
        sensitivityDb = settings.InputDeviceSensitivity;
        holdTimeMs = 400;
        adaptiveMarginDb = 10;
        zcrThreshold = 0.25f;
        zcrSpeechRmsThreshold = -25;
        holdTime = TimeSpan.FromMilliseconds(holdTimeMs);
    }

    private void OnSettingsChanged()
    {
        LoadSettings();
    }

    void IVoiceActivityDetector.Reset()
    {
        lock (@lock)
        {
            lastSpeechTime = DateTime.MinValue;
            recentRms.Clear();
            noiseFloor = -60f;
            noiseFloorInitialized = false;
        }
    }

    bool IVoiceActivityDetector.IsVoice(byte[] pcmData)
    {
        if (pcmData.Length % 2 != 0)
        {
            throw new ArgumentException("PCM data must have even length (16-bit samples)");
        }

        var samples = new short[pcmData.Length / 2];
        Buffer.BlockCopy(pcmData, 0, samples, 0, pcmData.Length);
        var rmsDb = ComputeRmsDb(samples);
        var isSpeech = rmsDb > GetCurrentThreshold();

        if (isSpeech)
        {
            var zcr = ComputeZeroCrossingRate(samples);
            if (zcr > zcrThreshold)
            {
                if (rmsDb > zcrSpeechRmsThreshold)
                {
                    isSpeech = true;
                }
                else
                {
                    isSpeech = false;
                }
            }
        }

        if (!isSpeech)
        {
            UpdateNoiseFloor(rmsDb);
        }

        if (isSpeech)
        {
            lastSpeechTime = DateTime.UtcNow;
            return true;
        }
        else
        {
            if ((DateTime.UtcNow - lastSpeechTime) < holdTime)
            {
                return true;
            }
            return false;
        }
    }

    private static float ComputeZeroCrossingRate(short[] samples)
    {
        var crossings = 0;
        for (var i = 1; i < samples.Length; i++)
        {
            if ((samples[i] > 0 && samples[i - 1] <= 0) || (samples[i] <= 0 && samples[i - 1] > 0))
            {
                crossings++;
            }
        }

        return (float)crossings / samples.Length;
    }

    private static float ComputeRmsDb(short[] samples)
    {
        long sum = 0;
        foreach (var s in samples)
        {
            sum += s * s;
        }

        var rms = Math.Sqrt((double)sum / samples.Length);
        return rms < 1e-10 ? -100f : (float)(20 * Math.Log10(rms / 32768.0));
    }

    private float GetCurrentThreshold()
        => noiseFloorInitialized ? noiseFloor + adaptiveMarginDb : sensitivityDb;
    private void UpdateNoiseFloor(float rmsDb)
    {
        lock (@lock)
        {
            recentRms.Enqueue(rmsDb);
            if (recentRms.Count > 100)
            {
                recentRms.Dequeue();
            }

            if (recentRms.Count >= 30 && recentRms.All(v => v < -50f))
            {
                noiseFloor = recentRms.Average();
                noiseFloorInitialized = true;
            }
        }
    }
}
