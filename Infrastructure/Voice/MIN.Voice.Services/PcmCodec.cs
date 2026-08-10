using MIN.Voice.Services.Contacts.Enums;
using MIN.Voice.Services.Contacts.Interfaces;

namespace MIN.Voice.Services;

/// <inheritdoc cref="IVoiceCodec"/>
public class PcmCodec : IVoiceCodec
{
    /// <inheritdoc />
    public VoiceCodecKind Kind => VoiceCodecKind.Pcm;

    /// <inheritdoc />
    public byte[] Encode(byte[] pcm) => pcm;

    /// <inheritdoc />
    public byte[] Decode(byte[] compressed) => compressed;
}
