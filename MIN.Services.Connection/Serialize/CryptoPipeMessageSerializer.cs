using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Connection.Contracts.Models.Enums;
using MIN.Services.Connection.Cryptographing;
using MIN.Services.Contracts.Models;

namespace MIN.Services.Connection.Serialize
{
    /// <summary>
    /// Обычный сериализатор сообщений
    /// </summary>
    public class CryptoPipeMessageSerializer : IPipeMessageSerializer
    {
        public readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private readonly ICryptoProvider cryptoProvider;

        /// <inheritdoc cref="IPipeMessageSerializer.RoomName"/>
        public string RoomName { get; set; } = string.Empty;

        public CryptoPipeMessageSerializer(ICryptoProvider cryptoProvider)
        {
            this.cryptoProvider = cryptoProvider;
        }

        // Формат сообщения: [4 байта: длина][1 байт: тип][N байт: данные]
        async Task<object> IPipeMessageSerializer.ReadMessageAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            cryptoProvider.RoomName = RoomName;

            // Читаем заголовок pipe
            var lengthBuffer = new byte[4];
            await stream.ReadExactlyAsync(lengthBuffer, cancellationToken);
            var length = BitConverter.ToInt32(lengthBuffer, 0);

            var typeBuffer = new byte[1];
            await stream.ReadExactlyAsync(typeBuffer, cancellationToken);
            var messageType = (MessageTypeTag)typeBuffer[0];

            var encryptedBuffer = new byte[length];
            await stream.ReadExactlyAsync(encryptedBuffer, cancellationToken);

            var decryptedBuffer = cryptoProvider.DecryptMessage(encryptedBuffer);

            return messageType switch
            {
                MessageTypeTag.ChatMessage =>
                    JsonSerializer.Deserialize<ChatMessage>(decryptedBuffer, JsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize ChatMessage"),

                MessageTypeTag.RoomInfo =>
                    JsonSerializer.Deserialize<RoomInfoMessage>(decryptedBuffer, JsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize RoomInfoMessage"),

                MessageTypeTag.DiscoveredRoom =>
                    JsonSerializer.Deserialize<DiscoveredRoom>(decryptedBuffer, JsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize DiscoveredRoom"),

                    _ => throw new InvalidDataException($"Unknown message type: {messageType}")
            };
        }

        async Task IPipeMessageSerializer.WriteMessageAsync<T>(Stream stream, T message, CancellationToken cancellationToken = default)
            where T : class
        {
            cryptoProvider.RoomName = RoomName;

            var json = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);

            var encryptedBuffer = cryptoProvider.EncryptMessage(json);

            var lengthBuffer = BitConverter.GetBytes(encryptedBuffer.Length);
            var typeTag = message switch
            {
                ChatMessage => MessageTypeTag.ChatMessage,
                RoomInfoMessage => MessageTypeTag.RoomInfo,
                DiscoveredRoom => MessageTypeTag.DiscoveredRoom,
                _ => throw new ArgumentException($"Unsupported type: {typeof(T).Name}")
            };

            await stream.WriteAsync(lengthBuffer, cancellationToken);
            await stream.WriteAsync(new[] { (byte)typeTag }, cancellationToken);
            await stream.WriteAsync(encryptedBuffer, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }
    }
}
