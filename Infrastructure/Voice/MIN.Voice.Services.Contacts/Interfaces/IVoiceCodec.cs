using MIN.Voice.Services.Contacts.Enums;

namespace MIN.Voice.Services.Contacts.Interfaces
{
    /// <summary>
    /// Кодировщик звука в байты
    /// </summary>
    public interface IVoiceCodec
    {
        /// <summary>
        /// Вид кодировки
        /// </summary>
        VoiceCodecKind Kind { get; }

        /// <summary>
        /// Закодировать звук в байты
        /// </summary>
        byte[] Encode(byte[] pcm);

        /// <summary>
        /// Распоковать байты в звук
        /// </summary>
        byte[] Decode(byte[] compressed);
    }
}
