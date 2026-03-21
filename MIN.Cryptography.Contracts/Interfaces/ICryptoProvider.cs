using MIN.Messaging.Contracts.Entities;
using MIN.Messaging.Stateless;

namespace MIN.Cryptography.Contracts.Interfaces
{
    /// <summary>
    /// Помошник в шифровании
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
        Task InitializeSessionAsync(Guid partnerId, byte[] partnerPublicKey);

        /// <summary>
        /// Создать сообщения рукопожатия
        /// </summary>
        Task<HandshakeMessage> CreateHandshakeAsync(ParticipantInfo selfParticipant);

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
