using System.Security.Cryptography;
using MIN.Services.Connection.Contracts.Models.Cryptographing;

namespace MIN.Services.Connection.Contracts.Interfaces.Cryptographing
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
        Task<byte[]> ComputeSharedSecretAsync(string partnerEcdhPublicKeyDerBase64);

        /// <summary>
        /// Сохранить публичный ключ собеседника (для TOFU)
        /// </summary>
        Task SavePartnerPublicKeyAsync(Guid partnerId, string ecdhPublicKeyPem);

        /// <summary>
        /// Получить сохранённый публичный ключ собеседника
        /// </summary>
        Task<string?> GetPartnerPublicKeyAsync(Guid partnerId);
    }
}
