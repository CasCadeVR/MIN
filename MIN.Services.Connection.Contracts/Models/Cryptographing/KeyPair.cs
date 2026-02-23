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
        public string EcdhPublicKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Зашифрованный приватный ключ для шифрования (ECDH)
        /// </summary>
        public string EncryptedEcdhPrivateKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Публичный ключ для подписи (RSA)
        /// </summary>
        public string RsaPublicKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Зашифрованный приватный ключ для подписи (RSA)
        /// </summary>
        public string EncryptedRsaPrivateKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Соль для HKDF (не секрет, но уникальная)
        /// </summary>
        public string SaltHex { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор ключей (для ротации)
        /// </summary>
        public Guid KeyId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
