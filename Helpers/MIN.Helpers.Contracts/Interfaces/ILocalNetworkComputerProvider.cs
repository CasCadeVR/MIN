namespace MIN.Helpers.Contracts.Interfaces;

/// <summary>
/// Сервис по работе с компьютерами в локальной сети
/// </summary>
public interface ILocalNetworkComputerProvider
{
    /// <summary>
    /// Получить имя локальной машины в сети
    /// </summary>
    string GetLocalMachineName();

    /// <summary>
    /// Получить список имён компьютеров
    /// </summary>
    IEnumerable<string> GetLocalNetworkMachineNames(string searchZone);
}
