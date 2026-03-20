namespace MIN.Cryptography.Contracts.Models
{
    /// <summary>
    /// Пара ключей
    /// </summary>
    public class KeyPair
    {
        /// <summary>
        /// Идентификатор ключей
        /// </summary>
        public Guid KeyId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Публичный ключ для шифрования (ECDH)
        /// </summary>
        public string EcdhPublicKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Зашифрованный приватный ключ для шифрования (ECDH)
        /// </summary>
        public string EncryptedEcdhPrivateKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Для сетевого обмена
        /// </summary>
        public byte[] EcdhPublicKeyBytes { get; set; } = null!;
    }
}
