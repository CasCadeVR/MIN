namespace MIN.Services.Connection.Contracts.Models.Cryptographing
{
    /// <summary>
    /// Пара ключей
    /// </summary>
    public class KeyPair
    {
        /// <summary>
        /// Асимметричные ключи (RSA)
        /// </summary>
        public string PublicKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Приватный ключ хранится ЗАШИФРОВАННЫМ (DPAPI)
        /// </summary>
        public string EncryptedPrivateKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Симметричный ключ для AES
        /// </summary>
        public string EncryptedAesKey { get; set; } = string.Empty;

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
