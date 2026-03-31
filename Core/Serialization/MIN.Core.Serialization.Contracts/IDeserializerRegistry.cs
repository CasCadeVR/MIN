using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;

namespace MIN.Core.Serialization.Contracts;

/// <summary>
/// Реестр десериализаторов сообщений
/// </summary>
public interface IDeserializerRegistry
{
    /// <summary>
    /// Зарегистрировать десериализатор
    /// </summary>
    void RegisterDeserializer(MessageTypeTag tag, Func<byte[], IMessage> deserializer);

    /// <summary>
    /// Получить десериализатор для указанного типа сообщения
    /// </summary>
    Func<byte[], IMessage>? GetDeserializer(MessageTypeTag tag);
}
