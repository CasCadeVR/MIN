using System.Text.Json.Serialization;

namespace MIN.Services.Connection.Contracts.Models.Cryptographing
{
    /// <summary>
    /// Пара ключей
    /// </summary>
    public class KeyPair
    {
        /// <summary>
        /// Публичный ключ для шифрования (ECDH)
        /// </summary>
        [JsonPropertyName("ecdhPublicKeyPem")]
        public string EcdhPublicKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Зашифрованный приватный ключ для шифрования (ECDH)
        /// </summary>
        [JsonPropertyName("encryptedEcdhPrivateKeyPem")]
        public string EncryptedEcdhPrivateKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Публичный ключ для подписи (RSA)
        /// </summary>
        [JsonPropertyName("rsaPublicKeyPem")]
        public string RsaPublicKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Зашифрованный приватный ключ для подписи (RSA)
        /// </summary>
        [JsonPropertyName("encryptedRsaPrivateKeyPem")]
        public string EncryptedRsaPrivateKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Для сетевого обмена
        /// </summary>
        [JsonPropertyName("ecdhPublicKeyDerBase64")]
        public string EcdhPublicKeyDerBase64 { get; set; } = string.Empty;

        /// <summary>
        /// Соль для HKDF (не секрет, но уникальная)
        /// </summary>
        [JsonPropertyName("saltHex")]
        public string SaltHex { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор ключей (для ротации)
        /// </summary>
        [JsonPropertyName("keyId")]
        public Guid KeyId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Дата создания
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
