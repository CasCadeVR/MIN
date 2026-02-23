using MIN.Services.Contracts.Models;

namespace MIN.Services.Connection.Contracts.Interfaces.Discovering
{
    /// <summary>
    /// Клиент обнаружения сервера
    /// </summary>
    public interface IDiscoveryClient
    {
        /// <summary>
        /// Обнаружить комнату
        /// </summary>
        Task<DiscoveredRoom?> DiscoverRoomAsync(string targetPCName, TimeSpan timeout);
    }
}
