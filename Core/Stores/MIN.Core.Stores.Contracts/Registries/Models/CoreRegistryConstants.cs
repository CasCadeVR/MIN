namespace MIN.Core.Stores.Contracts.Registries.Models;

/// <summary>
/// Константы уровня регистра соединений
/// </summary>
public class CoreRegistryConstants
{
    /// <summary>
    /// Локальное подключение - логический идентификатор, который представляет соединение с самим собой.
    /// </summary>
    public readonly static Guid LocalConnectionId = Guid.Empty;
}
