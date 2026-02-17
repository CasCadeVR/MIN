using System.IO.Pipes;
using MIN.Services.Contracts.Models;

namespace MIN.Services.Connection.Contracts.Models
{
    /// <summary>
    /// Подключения к клиенту
    /// </summary>
    public class ClientConnection(NamedPipeServerStream pipe) : IAsyncDisposable
    {
        public NamedPipeServerStream Pipe { get; } = pipe;
        public Participant Participant { get; set; } = null!;
        public Task ProcessingTask { get; set; } = null!;

        public async ValueTask DisposeAsync()
        {
            if (Pipe != null)
            {
                try { await Pipe.DisposeAsync(); } catch { }
            }
        }
    }
}
