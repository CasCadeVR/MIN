namespace MIN.Discovery.Transport.UdpBroadcast;

/// <summary>
/// Хелпер для упаковки/распаковки UDP-пакетов с magic bytes "MI" + длина
/// </summary>
internal static class UdpPacketHelper
{
    /// <summary>
    /// Максимальный размер полезной нагрузки UDP (MTU 1472 - заголовок 4)
    /// </summary>
    public const int MaxPayloadSize = 1468;

    private const int HeaderSize = 4;

    /// <summary>
    /// Упаковать payload в UDP-пакет (добавляет magic bytes + длину)
    /// </summary>
    public static byte[] Pack(byte[] payload)
    {
        if (payload.Length > MaxPayloadSize)
            throw new ArgumentException($"Payload exceeds UDP max ({MaxPayloadSize} bytes)");

        var packet = new byte[payload.Length + HeaderSize];
        packet[0] = (byte)'M';
        packet[1] = (byte)'I';
        packet[2] = (byte)((payload.Length >> 8) & 0xFF);
        packet[3] = (byte)(payload.Length & 0xFF);
        Array.Copy(payload, 0, packet, HeaderSize, payload.Length);
        return packet;
    }

    /// <summary>
    /// Распаковать UDP-пакет, проверив magic bytes
    /// </summary>
    public static bool TryUnpack(byte[] packet, out byte[] payload)
    {
        payload = [];
        if (packet.Length < HeaderSize)
            return false;
        if (packet[0] != 'M' || packet[1] != 'I')
            return false;

        var len = (packet[2] << 8) | packet[3];
        if (len + HeaderSize != packet.Length)
            return false;

        payload = new byte[len];
        Array.Copy(packet, HeaderSize, payload, 0, len);
        return true;
    }
}
