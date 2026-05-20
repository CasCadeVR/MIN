using System.Net;

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
        return [IPAddress.Broadcast];
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
