namespace MIN.Helpers.Contracts.Interfaces;

/// <summary>
/// Провайдер версии приложения
/// </summary>
public interface IVersionProvider
{
    /// <summary>
    /// Версия приложения
    /// </summary>
    Version Version { get; }
}
