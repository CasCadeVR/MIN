using MIN.Services.Contracts.Models.Messages;

namespace MIN.Services.Connection.Contracts.Interfaces.Cryptographing
{
    /// <summary>
    /// Помощник в шифровании
    /// </summary>
    public interface ICryptoProvider
    {
        /// <summary>
        /// Проверить, инициализирована ли сессия с собеседником
        /// </summary>
        bool IsSessionInitialized(Guid partnerId);

        /// <summary>
        /// Вызывается при первом контакте с собеседником
        /// </summary>
        Task InitializeSessionAsync(Guid partnerId, HandshakeMessage handshake);

        /// <summary>
        /// Создать сообщения рукопожатия
        /// </summary>
        Task<HandshakeMessage> CreateHandshakeAsync(Guid id);

        /// <summary>
        /// Закодировать сообщение
        /// </summary>
        byte[] EncryptMessage(byte[] data, Guid partnerId);

        /// <summary>
        /// Раскодировать сообщение
        /// </summary>
        byte[] DecryptMessage(byte[] encryptedData, Guid partnerId);
    }
}
