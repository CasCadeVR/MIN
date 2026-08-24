using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Core.Handlers.Contracts.Exceptions;

/// <summary>
/// Ошибка несоответсвия типа
/// </summary>
public class HandlerTypeMismatch : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="HandlerTypeMismatch"/>
    /// </summary>
    public HandlerTypeMismatch(IMessageHandler handler, IMessage message)
        : base($"Неизвестный тип сообщения в {nameof(handler)} - {message.GetType()}") { }
}
