namespace MIN.Voice.Services.Contacts.Constants;

/// <summary>
/// Константы передачи звука
/// </summary>
public class VoiceAudioConstants
{
    /// <summary>
    /// хз
    /// </summary>
    public const int SampleRate = 16_000;

    /// <summary>
    /// хз
    /// </summary>
    public const int Channels = 1;

    /// <summary>
    /// хз
    /// </summary>
    public const int BitsPerSample = 16;

    /// <summary>
    /// хз
    /// </summary>
    public const int FrameDurationMs = 20;

    /// <summary>
    /// Байтов ща каждый фрейм
    /// </summary>
    public const int BytesPerFrame = SampleRate * Channels * (BitsPerSample / 8) * FrameDurationMs / 1000; // 640
}
