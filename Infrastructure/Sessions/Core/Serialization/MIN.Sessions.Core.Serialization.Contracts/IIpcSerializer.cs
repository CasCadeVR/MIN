using MIN.Sessions.Core.Messaging.Contracts.Models;

namespace MIN.Sessions.Core.Serialization.Contracts;

/// <summary>
/// Сериализатор междупроцессорных сообщений
/// </summary>
public interface IIpcSerializer
{
    /// <summary>
    /// Сериализовать междупроцессорное сообщение
    /// </summary>
    byte[] Serialize(IpcMessage message);

    /// <summary>
    /// Десериализовать междупроцессорное сообщение
    /// </summary>
    IpcMessage Deserialize(byte[] data);
}
