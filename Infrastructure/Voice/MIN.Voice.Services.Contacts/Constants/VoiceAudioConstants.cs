namespace MIN.Voice.Services.Contacts.Constants;

/// <summary>
/// Константы передачи звука
/// </summary>
public class VoiceAudioConstants
{
    /// <summary>
    /// Частота дискретизации в герцах (16 кГц).
    /// Стандартное значение для речевых сигналов, обеспечивающее хорошее качество
    /// при низкой задержке и умеренном объёме данных.
    /// </summary>
    public const int SampleRate = 16_000;

    /// <summary>
    /// Количество аудиоканалов (1 — моно).
    /// Моно достаточно для голосовой связи и позволяет сократить трафик вдвое по сравнению со стерео.
    /// </summary>
    public const int Channels = 1;

    /// <summary>
    /// Количество бит на один сэмпл (16 бит).
    /// Обеспечивает стандартное качество звука PCM (динамический диапазон ~96 дБ).
    /// </summary>
    public const int BitsPerSample = 16;

    /// <summary>
    /// Длительность одного аудиофрейма в миллисекундах.
    /// 20 мс — это общепринятый компромисс между задержкой (latency) и вычислительной эффективностью.
    /// </summary>
    public const int FrameDurationMs = 20;

    /// <summary>
    /// Размер одного фрейма в байтах, вычисляемый по формуле:
    /// SampleRate * Channels * (BitsPerSample / 8) * FrameDurationMs / 1000.
    /// Для текущих параметров (16 кГц, моно, 16 бит, 20 мс) даёт 640 байт.
    /// Используется для выделения буферов и пофреймовой обработки.
    /// </summary>
    public const int BytesPerFrame = SampleRate * Channels * (BitsPerSample / 8) * FrameDurationMs / 1000;
}
