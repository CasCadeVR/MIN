namespace MIN.Services.Contracts.Models.Messages
{
    /// <summary>
    /// Сообщения для обмена криптографической информации
    /// </summary>
    public class HandshakeMessage
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ECDH Публичный ключ
        /// </summary>
        public string EcdhPublicKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Публичный ключ RSA
        /// </summary>
        public string RsaPublicKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Штамп времени
        /// </summary>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}
