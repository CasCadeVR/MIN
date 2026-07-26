using Open.Nat;

namespace MIN.Core.Transport.Contracts.Helpers;

/// <summary>
/// Помошник в пробросе порта с использованием библиотеки Open.NAT
/// </summary>
public static class OpenNatHelper
{
    private static NatDevice? device;

    /// <summary>
    /// Получить первый рабочий Device чтобы на нём можно было развернуть UPnP
    /// </summary>
    public static async Task<NatDevice?> GetDeviceAsync(int timeoutMs = 5000)
    {
        if (device != null)
        {
            return device;
        }

        using var cts = new CancellationTokenSource(timeoutMs);
        var devices = await new NatDiscoverer().DiscoverDevicesAsync(PortMapper.Upnp, cts);

        // Trying to get working external IP
        foreach (var potentialDevice in devices)
        {
            try
            {
                var ip = await potentialDevice.GetExternalIPAsync();
                device = potentialDevice;
                return device;
            }
            catch { continue; }
        }
        return null;
    }
}
