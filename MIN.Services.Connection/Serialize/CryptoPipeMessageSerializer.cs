using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Services.Connection.Contracts.Interfaces.Cryptographing;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Connection.Contracts.Models.Enums;
using MIN.Services.Contracts.Constants;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Messages;

namespace MIN.Services.Connection.Serialize
{
    /// <summary>
    /// Обычный сериализатор сообщений
    /// </summary>
    public class CryptoPipeMessageSerializer : IPipeMessageSerializer
    {
        private readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private readonly ICryptoProvider cryptoProvider;

        public CryptoPipeMessageSerializer(ICryptoProvider cryptoProvider)
        {
            this.cryptoProvider = cryptoProvider;
        }

        // Формат сообщения: [4 байта: длина][1 байт: тип][N байт: данные]
        async Task<object> IPipeMessageSerializer.ReadMessageAsync(Stream stream, Guid senderId, CancellationToken cancellationToken = default)
        {
            var lengthBuffer = new byte[4];
            await stream.ReadExactlyAsync(lengthBuffer, cancellationToken);
            var length = BitConverter.ToInt32(lengthBuffer, 0);

            var typeBuffer = new byte[1];
            await stream.ReadExactlyAsync(typeBuffer, cancellationToken);
            var messageType = (MessageTypeTag)typeBuffer[0];

            var dataBuffer = new byte[length];
            await stream.ReadExactlyAsync(dataBuffer, cancellationToken);

            switch (messageType)
            {
                case MessageTypeTag.HandshakeMessage:
                    return JsonSerializer.Deserialize<HandshakeMessage>(dataBuffer, JsonOptions)
                        ?? throw new InvalidDataException("Failed to deserialize HandshakeMessage");

                case MessageTypeTag.DiscoveredRoom:
                    return JsonSerializer.Deserialize<DiscoveredRoom>(dataBuffer, JsonOptions)
                        ?? throw new InvalidDataException("Failed to deserialize DiscoveredRoom");

                case MessageTypeTag.ChatMessage:
                    return await DecryptAndDeserializeAsync<ChatMessage>(dataBuffer, senderId);

                case MessageTypeTag.RoomInfo:
                    return await DecryptAndDeserializeAsync<RoomInfoMessage>(dataBuffer, senderId);

                case MessageTypeTag.RoomInfoRequest:
                    return await DecryptAndDeserializeAsync<RoomInfoRequestMessage>(dataBuffer, senderId);

                default:
                    throw new InvalidDataException($"Unknown message type: {messageType}");
            }
        }

        async Task IPipeMessageSerializer.WriteMessageAsync<T>(Stream stream, T message, Guid recipientId, CancellationToken cancellationToken = default)
            where T : class
        {
            byte[] payload;
            MessageTypeTag messageType;

            if (message is HandshakeMessage handshake)
            {
                payload = JsonSerializer.SerializeToUtf8Bytes(handshake, JsonOptions);
                messageType = MessageTypeTag.HandshakeMessage;
            }
            else if (message is DiscoveredRoom discoveredRoom)
            {
                payload = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);
                messageType = MessageTypeTag.DiscoveredRoom;
            }
            else
            {
                var json = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);
                if (json.Length > ChatMessageConstants.MaximumMessageSize)
                {
                    throw new ArgumentException("Message too large", nameof(message));
                }

                payload = cryptoProvider.EncryptMessage(json, recipientId);
                messageType = message switch
                {
                    ChatMessage => MessageTypeTag.ChatMessage,
                    RoomInfoMessage => MessageTypeTag.RoomInfo,
                    DiscoveredRoom => MessageTypeTag.DiscoveredRoom,
                    HandshakeMessage => MessageTypeTag.HandshakeMessage,
                    RoomInfoRequestMessage => MessageTypeTag.RoomInfoRequest,
                    _ => throw new ArgumentException($"Unsupported message type: {typeof(T).Name}")
                };
            }

            // 4. Пишем в stream: [длина][тип][данные]
            var lengthBuffer = BitConverter.GetBytes(payload.Length);
            await stream.WriteAsync(lengthBuffer, cancellationToken);
            await stream.WriteAsync(new[] { (byte)messageType }, cancellationToken);
            await stream.WriteAsync(payload, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }

        private async Task<T> DecryptAndDeserializeAsync<T>(
            byte[] encryptedData,
            Guid senderId) where T : class
        {
            if (!cryptoProvider.IsSessionInitialized(senderId))
            {
                throw new InvalidOperationException(
                    $"Session with '{senderId}' not initialized. Wait for Handshake first.");
            }

            var decrypted = cryptoProvider.DecryptMessage(encryptedData, senderId);
            return JsonSerializer.Deserialize<T>(decrypted, JsonOptions)
                ?? throw new InvalidDataException($"Failed to deserialize {typeof(T).Name}");
        }
    }
}
