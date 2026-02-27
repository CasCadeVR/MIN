using System.Text.Json.Serialization;

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
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// ECDH Публичный ключ
        /// </summary>
        [JsonPropertyName("ecdhPublicKeyDerBase64")]
        public string EcdhPublicKeyDerBase64 { get; set; } = string.Empty;

        /// <summary>
        /// Публичный ключ RSA
        /// </summary>
        [JsonPropertyName("rsaPublicKeyPem")]
        public string RsaPublicKeyPem { get; set; } = string.Empty;

        /// <summary>
        /// Штамп времени
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}
