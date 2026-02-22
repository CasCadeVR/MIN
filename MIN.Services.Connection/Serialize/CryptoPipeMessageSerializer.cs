using System.Text.Json;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Connection.Contracts.Models.Enums;
using MIN.Services.Contracts.Constants;
using MIN.Services.Contracts.Models;

namespace MIN.Services.Connection.Serialize
{
    /// <summary>
    /// Обычный сериализатор сообщений
    /// </summary>
    public class CryptoPipeMessageSerializer : IPipeMessageSerializer
    {
        private readonly CommonPipeMessageSerializer innerSerializer;
        private readonly ICryptoProvider cryptoProvider;

        public CryptoPipeMessageSerializer(
           CommonPipeMessageSerializer innerSerializer,
           ICryptoProvider cryptoProvider)
        {
            this.innerSerializer = innerSerializer;
            this.cryptoProvider = cryptoProvider;
        }

        // Формат сообщения: [4 байта: длина][1 байт: тип][N байт: данные]
        async Task<object> IPipeMessageSerializer.ReadMessageAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var lengthBuffer = new byte[4];
            await stream.ReadExactlyAsync(lengthBuffer, cancellationToken);
            var length = BitConverter.ToInt32(lengthBuffer, 0);

            if (length <= 0 || length > ChatMessageConstants.MaximumMessageSize)
                throw new InvalidDataException($"Invalid message size: {length}");

            var typeBuffer = new byte[1];
            await stream.ReadExactlyAsync(typeBuffer, cancellationToken);
            var messageType = (MessageTypeTag)typeBuffer[0];

            var encryptedBuffer = new byte[length];
            await stream.ReadExactlyAsync(encryptedBuffer, cancellationToken);

            var decryptedBuffer = await cryptoProvider.DecryptMessageAsync(encryptedBuffer);

            return messageType switch
            {
                MessageTypeTag.ChatMessage =>
                    JsonSerializer.Deserialize<ChatMessage>(decryptedBuffer, innerSerializer.JsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize ChatMessage"),

                MessageTypeTag.RoomInfo =>
                    JsonSerializer.Deserialize<RoomInfoMessage>(decryptedBuffer, innerSerializer.JsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize RoomInfoMessage"),

                MessageTypeTag.DiscoveredRoom =>
                    JsonSerializer.Deserialize<DiscoveredRoom>(decryptedBuffer, innerSerializer.JsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize DiscoveredRoom"),

                    _ => throw new InvalidDataException($"Unknown message type: {messageType}")
            };
        }

        async Task IPipeMessageSerializer.WriteMessageAsync<T>(
            Stream stream, T message, CancellationToken cancellationToken = default)
            where T : class
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(message, innerSerializer.JsonOptions);

            var encryptedPayload = await cryptoProvider.EncryptMessageAsync(json);

            var typeTag = message switch
            {
                ChatMessage => MessageTypeTag.ChatMessage,
                RoomInfoMessage => MessageTypeTag.RoomInfo,
                DiscoveredRoom => MessageTypeTag.DiscoveredRoom,
                _ => throw new ArgumentException($"Unsupported message type: {typeof(T).Name}")
            };

            // 4. Пишем в том же формате: [длина][тип][данные]
            var lengthBuffer = BitConverter.GetBytes(encryptedPayload.Length);
            await stream.WriteAsync(lengthBuffer, cancellationToken);
            await stream.WriteAsync(new[] { (byte)typeTag }, cancellationToken);
            await stream.WriteAsync(encryptedPayload, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }
    }
}
