using System.IO.Pipes;
using MIN.Services.Contracts.Models;

namespace MIN.Services.Connection.Contracts.Models
{
    /// <summary>
    /// Слот подключения сервера и клиента
    /// </summary>
    public class ClientConnection(NamedPipeServerStream pipe) : IAsyncDisposable
    {
        /// <summary>
        /// Pipe по которой идёт обмен данных
        /// </summary>
        public NamedPipeServerStream Pipe { get; } = pipe;

        /// <summary>
        /// Участник
        /// </summary>
        public Participant Participant { get; set; } = new Participant();

        /// <summary>
        /// Фоновая обработка операция для клиента
        /// </summary>
        public Task ProcessingTask { get; set; } = null!;

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
        public async ValueTask DisposeAsync()
        {
            if (Pipe != null)
            {
                try
                {
                    await Pipe.DisposeAsync();
                } 
                catch { }
            }
        }
    }
}
