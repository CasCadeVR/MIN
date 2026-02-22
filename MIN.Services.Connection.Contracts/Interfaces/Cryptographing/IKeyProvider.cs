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
        /// Получить локальные ключи текущего пользователя
        /// </summary>
        Task<KeyPair> GetLocalKeysAsync();

        /// <summary>
        /// Сохранить публичный ключ собеседника (для шифрования ему)
        /// </summary>
        Task SavePartnerPublicKeyAsync(string partnerId, string publicKeyPem);

        /// <summary>
        /// Получить публичный ключ собеседника (для шифрования ему)
        /// </summary>
        Task<string?> GetPartnerPublicKeyAsync(string partnerId);

        /// <summary>
        /// Расшифровать приватный ключ (внутри KeyProvider, не нарушая инкапсуляции)
        /// </summary>
        Task<RSA> GetRsaPrivateKeyAsync();

        /// <summary>
        /// Получить AES-ключ для симметричного шифрования
        /// </summary>
        Task<byte[]> GetAesKeyAsync();

        /// <summary>
        /// Получить соль для HKDF
        /// </summary>
        Task<byte[]> GetSaltAsync();
    }
}
