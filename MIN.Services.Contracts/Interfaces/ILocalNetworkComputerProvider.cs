namespace MIN.Services.Contracts.Interfaces
{
    public interface ILocalNetworkComputerProvider
    {
        /// <summary>
        /// Получить список имён компьютеров
        /// </summary>
        IEnumerable<string> GetLocalNetworkComputerNames(string searchZone);
    }
}
