namespace MIN.Helpers.Contracts.Interfaces;

/// <summary>
/// Сервис по работе с хранением в файлах
/// </summary>
public interface IAppDataProvider
{
    /// <summary>
    /// Корневая папка
    /// </summary>
    string BaseDirectory { get; }
}
