using System.Net;
using System.Net.Sockets;
using MIN.Discovery.Transport.Contracts;
using MIN.Discovery.Transport.UdpBroadcast.Helpers;

namespace MIN.Discovery.Transport.UdpBroadcast.Models;

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
