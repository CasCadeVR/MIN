namespace MIN.Services.Connection.Contracts.Interfaces.Discovering
{
    /// <summary>
    /// Сервер для обнаружения
    /// </summary>
    public interface IDiscoveryServer
    {
        /// <summary>
        /// Запустить сервер
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Остановить сервер
        /// </summary>
        Task StopAsync();
    }
}
