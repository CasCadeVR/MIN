using System.Net;
using System.Net.Sockets;

namespace MIN.Core.Transport.TcpSockets.Services;

internal sealed class RoomPortManager : IDisposable
{
    private readonly int minPort = 49152;
    private readonly int maxPort = 65535;
    private readonly HashSet<int> reserved = [];
    private readonly Random random = new();

    /// <summary>
    /// Получить свободный порт
    /// </summary>
    public int AllocatePort()
    {
        for (var i = 0; i < 50; i++)
        {
            var port = random.Next(minPort, maxPort + 1);
            if (!reserved.Contains(port) && IsPortFree(port))
            {
                reserved.Add(port);
                return port;
            }
        }
        throw new InvalidOperationException("No free port in dynamic range");
    }

    /// <summary>
    /// Отпустить порт
    /// </summary>
    public void ReleasePort(int port) => reserved.Remove(port);

    private static bool IsPortFree(int port)
    {
        try
        {
            using var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listener.Stop();
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => reserved.Clear();
}
