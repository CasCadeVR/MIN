using System.Net;
using System.Net.Sockets;
using MIN.Discovery.Transport.Contracts;

namespace MIN.Discovery.Transport.UdpBroadcast;

internal sealed class UdpDiscoveryResponder : IDiscoveryResponder
{
    private readonly UdpClient client;
    private readonly IPEndPoint remoteEndPoint;

    public UdpDiscoveryResponder(UdpClient client, IPEndPoint remoteEndPoint)
    {
        this.client = client;
        this.remoteEndPoint = remoteEndPoint;
    }

    public async Task RespondAsync(byte[] data, CancellationToken ct = default)
    {
        var packet = UdpPacketHelper.Pack(data);
        await client.SendAsync(packet, packet.Length, remoteEndPoint);
    }
}
