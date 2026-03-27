namespace MIN.Helpers.Contracts.Interfaces;

/// <summary>
/// Сервис по работе с компьютерами в локальной сети
/// </summary>
public interface ILocalNetworkComputerProvider
{
    /// <summary>
    /// Получить список имён компьютеров
    /// </summary>
    IEnumerable<string> GetLocalNetworkComputerNames(string searchZone);
}
