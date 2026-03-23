using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Connection.Contracts.Models.Enums;
using MIN.Services.Contracts.Constants;
using MIN.Services.Contracts.Models;
using MIN.Services.Contracts.Models.Messages;

namespace MIN.Serialization.Json
{
    /// <summary>
    /// Обычный сериализатор сообщений
    /// </summary>
    public class CommonPipeMessageSerializer : IPipeMessageSerializer
    {
        /// <inheritdoc cref="IPipeMessageSerializer.RoomName"/>
        public string RoomName { get; set; } = string.Empty;

        public readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // Формат сообщения: [4 байта: длина][1 байт: тип][N байт: данные]
        async Task<object> IPipeMessageSerializer.ReadMessageAsync(Stream stream, Guid senderId, CancellationToken cancellationToken)
        {
            // Читаем длину
            var lengthBuffer = new byte[4];
            await stream.ReadExactlyAsync(lengthBuffer, cancellationToken);
            var length = BitConverter.ToInt32(lengthBuffer, 0);

            if (length <= 0 || length > ChatMessageConstants.MaximumMessageSize)
            {
                throw new InvalidDataException($"Invalid message size: {length}");
            }

            // Читаем тип сообщения
            var typeBuffer = new byte[1];
            await stream.ReadExactlyAsync(typeBuffer, cancellationToken);
            var messageType = (MessageTypeTag)typeBuffer[0];

            // Читаем данные
            var dataBuffer = new byte[length];
            await stream.ReadExactlyAsync(dataBuffer, cancellationToken);

            return messageType switch
            {
                MessageTypeTag.ChatMessage =>
                    JsonSerializer.Deserialize<ChatMessage>(dataBuffer, JsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize ChatMessage"),

                MessageTypeTag.HandshakeMessage =>
                    JsonSerializer.Deserialize<HandshakeMessage>(dataBuffer, JsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize HandshakeMessage"),

                MessageTypeTag.RoomInfo =>
                    JsonSerializer.Deserialize<RoomInfoMessage>(dataBuffer, JsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize RoomInfoMessage"),

                MessageTypeTag.DiscoveredRoom =>
                    JsonSerializer.Deserialize<DiscoveredRoom>(dataBuffer, JsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize DiscoveredRoom"),

                _ => throw new InvalidDataException($"Unknown message type: {messageType}")
            };
        }

        async Task IPipeMessageSerializer.WriteMessageAsync<T>(Stream stream, T message, Guid recipientId, CancellationToken cancellationToken)
            where T : class
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);
            if (json.Length > ChatMessageConstants.MaximumMessageSize)
            {
                throw new ArgumentException("Слишком большое сообщение", nameof(message));
            }

            // Определяем тип для заголовка
            var typeTag = message switch
            {
                ChatMessage => MessageTypeTag.ChatMessage,
                RoomInfoMessage => MessageTypeTag.RoomInfo,
                DiscoveredRoom => MessageTypeTag.DiscoveredRoom,
                HandshakeMessage => MessageTypeTag.HandshakeMessage,
                _ => throw new ArgumentException($"Неподдерживаемый тип сообщения: {typeof(T).Name}")
            };

            // Записываем длину
            var lengthBuffer = BitConverter.GetBytes(json.Length);
            await stream.WriteAsync(lengthBuffer, cancellationToken);

            // Записываем тип
            await stream.WriteAsync(new[] { (byte)typeTag }, cancellationToken);

            // Записываем данные
            await stream.WriteAsync(json, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }
    }
}
