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

    private IEnumerable<IPAddress> GetAllBroadcastChannels()
    {
        var result = new List<IPAddress>();

        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != OperationalStatus.Up ||
                ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
            {
                continue;
            }

            var props = ni.GetIPProperties();

            if (props.GatewayAddresses.Count > 0)
            {
                Console.WriteLine($"Главный адаптер: {ni.Description}");
                foreach (var ip in props.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        Console.WriteLine($"Его IP: {ip.Address}");
                        var ipAddr = BitConverter.ToUInt32(ip.Address.GetAddressBytes(), 0);
                        var mask = BitConverter.ToUInt32(ip.IPv4Mask.GetAddressBytes(), 0);
                        var broadcast = ipAddr | ~mask;
                        result.Add(new IPAddress(BitConverter.GetBytes(broadcast)));
                    }
                }
            }
        }

        return result;
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
