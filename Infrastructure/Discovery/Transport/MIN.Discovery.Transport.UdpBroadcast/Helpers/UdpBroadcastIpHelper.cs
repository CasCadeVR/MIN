using System.Net;
using System.Net.NetworkInformation;

namespace MIN.Discovery.Transport.UdpBroadcast.Helpers;

/// <summary>
/// Хелпер для резолвинга широковещательных каналов в локальной сети
/// </summary>
internal class UdpBroadcastIpHelper : IDisposable
{
    private readonly UdpBroadcastIpStorage storage;
    private IEnumerable<IPAddress>? cachedAddresses;
    private readonly SemaphoreSlim cacheLock = new(1, 1);
    private bool disposed;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="UdpBroadcastIpHelper"/>
    /// </summary>
    public UdpBroadcastIpHelper(UdpBroadcastIpStorage storage)
    {
        this.storage = storage;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IPAddress>> GetBroadcastAddressesAsync()
    {
        if (cachedAddresses != null)
        {
            return cachedAddresses;
        }

        await cacheLock.WaitAsync();
        try
        {
            if (cachedAddresses != null)
            {
                return cachedAddresses;
            }

            cachedAddresses = await storage.LoadBroadcastAddressesAsync();

            if (cachedAddresses == null)
            {
                cachedAddresses = GetAllBroadcastChannels();
                await storage.SaveBroadcastAddressesAsync(cachedAddresses);
            }

            return cachedAddresses;
        }
        finally
        {
            cacheLock.Release();
        }
    }

    private static List<IPAddress> GetAllBroadcastChannels()
    {
        var result = new HashSet<IPAddress>
        {
            // Always include the limited broadcast as a fallback
            IPAddress.Broadcast // 255.255.255.255
        };

        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != OperationalStatus.Up ||
                ni.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel ||
                ni.NetworkInterfaceType == NetworkInterfaceType.Ppp ||
                !ni.Supports(NetworkInterfaceComponent.IPv4))
            {
                continue;
            }

            var props = ni.GetIPProperties();

            foreach (var ip in props.UnicastAddresses)
            {
                if (ip.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    continue;
                }

                if (ip.IPv4Mask == null || ip.PrefixLength >= 31)
                {
                    // /31, /32 or missing mask -> point-to-point or invalid, skip
                    continue;
                }

                var ipAddr = BitConverter.ToUInt32(ip.Address.GetAddressBytes(), 0);
                var mask = BitConverter.ToUInt32(ip.IPv4Mask.GetAddressBytes(), 0);
                var broadcast = ipAddr | ~mask;
                result.Add(new IPAddress(BitConverter.GetBytes(broadcast)));
            }
        }

        return result.ToList();
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        cacheLock.Dispose();
        disposed = true;
    }
}
