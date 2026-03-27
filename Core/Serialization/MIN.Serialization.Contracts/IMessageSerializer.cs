using MIN.Messaging.Contracts.Interfaces;

namespace MIN.Serialization.Contracts;

/// <summary>
/// Сериализует и десериализует сообщения
/// </summary>
public interface IMessageSerializer
{
    /// <summary>
    /// Сериализует сообщение в массив байтов
    /// </summary>
    byte[] Serialize(IMessage message);

    /// <summary>
    /// Десериализует массив байтов в сообщение
    /// </summary>
    IMessage Deserialize(byte[] data);
}
