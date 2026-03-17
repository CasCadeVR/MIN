using System.Security.Cryptography;
using MIN.Cryptography.Contracts.Models;

namespace MIN.Cryptography.Contracts.Interfaces
{
    /// <summary>
    /// Помощник для хранения ключей
    /// </summary>
    public interface IKeyProvider
    {
        /// <summary>
        /// Получить или сгенерировать локальную пару ключей
        /// </summary>
        Task<KeyPair> GetLocalKeysAsync();

        /// <summary>
        /// Получить приватный ECDH-ключ для вычисления общего секрета
        /// </summary>
        Task<ECDiffieHellman> GetEcdhPrivateKeyAsync();

        /// <summary>
        /// Вычислить общий секрет с собеседником по его публичному ECDH-ключу
        /// </summary>
        Task<byte[]> ComputeSharedSecretAsync(byte[] partnerPublicKeyBytes);

        /// <summary>
        /// Сохранить публичный ключ собеседника (для TOFU)
        /// </summary>
        Task SavePartnerPublicKeyAsync(Guid partnerId, byte[] partnerPublicKeyBytes);

        /// <summary>
        /// Получить сохранённый публичный ключ собеседника
        /// </summary>
        Task<byte[]?> GetPartnerPublicKeyAsync(Guid partnerId);
    }
}
