using MIN.Core.Headers.Contracts.Constants;
using MIN.Core.Headers.Contracts.Enums;
using MIN.Core.Headers.Contracts.Interfaces;
using MIN.Core.Headers.Contracts.Models;

namespace MIN.Core.Headers.Services;

/// <summary>
/// <inheritdoc cref="IHeaderManager"/>
/// </summary>
public class HeaderManager : IHeaderManager
{
    byte[] IHeaderManager.AddHeader(byte[] data, byte header)
    {
        var result = new byte[1 + data.Length];
        result[0] = header;
        data.CopyTo(result, 1);
        return result;
    }

    byte[] IHeaderManager.RemoveEncryptionHeader(byte[] data)
        => data.Length > 1 ? data.AsSpan(1).ToArray() : [];

    HeaderMessageType IHeaderManager.GetMessageType(byte[] data)
    {
        if (data.Length == 0)
        {
            return HeaderMessageType.Plain;
        }
        return (HeaderMessageType)(data[0] & 0x8F);
    }

    bool IHeaderManager.IsEncrypted(byte[] data)
        => data.Length > 0 && (data[0] & (byte)HeaderMessageType.Encrypted) != 0;

    bool IHeaderManager.IsAck(byte[] data)
        => data.Length > 0 && (data[0] & (byte)HeaderMessageType.Ack) != 0;

    bool IHeaderManager.IsStreamChunk(byte[] data)
        => data.Length > 0 && (data[0] & (byte)HeaderMessageType.StreamChunk) != 0;

    /// <inheritdoc />
    public StreamChunkFlags GetStreamFlags(byte[] data)
    {
        if (data.Length == 0)
        {
            return StreamChunkFlags.None;
        }
        return (StreamChunkFlags)(data[0] & 0x0F);
    }

    byte[] IHeaderManager.BuildStreamChunkHeader(StreamChunkFlags flags, Guid streamId, int index, int total)
    {
        var header = new byte[HeadersConstants.StreamHeaderSize];
        header[0] = (byte)((byte)HeaderMessageType.StreamChunk | (byte)flags);
        streamId.TryWriteBytes(new Span<byte>(header, 1, 16));
        BitConverter.GetBytes(index).CopyTo(header, 17);
        BitConverter.GetBytes(total).CopyTo(header, 21);

        return header;
    }

    StreamChunkHeader IHeaderManager.ParseStreamChunkHeader(byte[] data)
        => new(
            GetStreamFlags(data),
            new Guid(data.AsSpan(1, 16)),
            BitConverter.ToInt32(data, 17),
            BitConverter.ToInt32(data, 21)
        );
}
