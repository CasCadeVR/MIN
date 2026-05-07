namespace MIN.Core.Messaging.Contracts.Interfaces;

/// <summary>
/// Сообщение с чувствительными полями
/// </summary>
public interface IMessageWithSecuredFields
{
    /// <summary>
    /// Очистить поля
    /// </summary>
    void Sanitize();
}
