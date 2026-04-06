using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.Contracts.Enum;
using Microsoft.Extensions.DependencyInjection;

namespace MIN.Core.Transport.NamedPipes.Factories
{
    /// <summary>
    /// Simple factory to create transports based on TransportType.
    /// </summary>
    public class NamedPipeTransportFactory : ITransportFactory
    {
        private readonly IServiceProvider provider;

        /// <summary>
        /// »нициализирует новый экземпл€р <see cref="NamedPipeTransportFactory"/>
        /// </summary>
        public NamedPipeTransportFactory(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public ITransport CreateTransport(TransportType type)
        {
            return type switch
            {
                TransportType.NamedPipe => provider.GetRequiredService<ITransport>(),
                _ => throw new NotSupportedException($"Transport type '{type}' is not supported yet.")
            };
        }
    }
}
