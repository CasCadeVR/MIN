using System.Diagnostics;
using System.Net;
using Open.Nat;

namespace MIN.Core.Transport.TcpSockets.Services;

internal class PortForwardingHelper
{
    private const string PublicIpResolverServiceUrl = "https://api.ipify.org";
    private NatDevice? device;
    private Mapping? activeMapping;

    /// <summary>
    /// Найти маршрутизатор и создать проброс порта
    /// </summary>
    /// <param name="privateIp">Приватный ip адрес</param>
    /// <param name="localPort">Ваш локальный порт (например, 56784)</param>
    /// <param name="publicPort">Желаемый публичный порт (если null, будет использован тот же)</param>
    /// <param name="description">Описание для правила (например, "Game room fgj")</param>
    /// <returns>Публичный IP-адрес или null, если не удалось</returns>
    public async Task<IPAddress?> CreatePortForwardingAsync(IPAddress privateIp, int localPort, int? publicPort = null, string description = "MIN TCP Server")
    {
        try
        {
            var discoverer = new NatDiscoverer();
            device = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, new CancellationTokenSource(5000));

            var publicIp = await GetPublicIpAsync();

            var mappingPort = publicPort ?? localPort;
            activeMapping = new Mapping(Protocol.Tcp, privateIP: privateIp, localPort, mappingPort, 0, description);
            Debug.WriteLine($"Created: {activeMapping.PublicPort} -> {activeMapping.PrivateIP}:{activeMapping.PrivatePort}");

            await device.CreatePortMapAsync(activeMapping);

            return IPAddress.Parse(publicIp);
        }
        catch
        {
            // UPnP не поддерживается, отключён или таймаут
            return null;
        }
    }

    /// <summary>
    /// Получить публичный адрес
    /// </summary>
    public static async Task<string> GetPublicIpAsync()
    {
        using var http = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };
        return await http.GetStringAsync(PublicIpResolverServiceUrl);
    }

    /// <summary>
    /// Удалить созданный проброс порта
    /// </summary>
    public async Task RemovePortForwardingAsync()
    {
        if (device != null && activeMapping != null)
        {
            try
            {
                await device.DeletePortMapAsync(activeMapping);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete mapping: {ex.Message}");
            }
        }
    }
}
