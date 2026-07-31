namespace MIN.Core.Entities.Contracts.Enums;

/// <summary>
/// Роль пользователя
/// </summary>
public enum Role
{
    /// <summary>
    /// Клиент (участник) - тот, кто подключается к комнате
    /// </summary>
    Client,

    /// <summary>
    /// Хост - тот, кто создал комнату
    /// </summary>
    Host
}
