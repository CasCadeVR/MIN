using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Services.Connection.Contracts.Interfaces.Serialize;
using MIN.Services.Contracts.Constants;
using MIN.Services.Contracts.Models;

namespace MIN.Services.Connection.Serialize
{
    /// <summary>
    /// Сериализатор сообщений
    /// </summary>
    public class PipeMessageSerializer : IPipeMessageSerializer
    {
        private readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // Формат сообщения: [4 байта: длина][1 байт: тип][N байт: данные]

        async Task<object> IPipeMessageSerializer.ReadMessageAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            // Читаем длину
            var lengthBuffer = new byte[4];
            await stream.ReadExactlyAsync(lengthBuffer, cancellationToken);
            var length = BitConverter.ToInt32(lengthBuffer, 0);

            if (length <= 0 || length > ChatMessageConstants.MaximumMessageSize)
                throw new InvalidDataException($"Invalid message size: {length}");

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
                    JsonSerializer.Deserialize<ChatMessage>(dataBuffer, jsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize ChatMessage"),

                MessageTypeTag.RoomInfo =>
                    JsonSerializer.Deserialize<RoomInfoMessage>(dataBuffer, jsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize RoomInfoMessage"),

                MessageTypeTag.DiscoveredRoom =>
                    JsonSerializer.Deserialize<DiscoveredRoom>(dataBuffer, jsonOptions)
                    ?? throw new InvalidDataException("Failed to deserialize DiscoveredRoom"),

                _ => throw new InvalidDataException($"Unknown message type: {messageType}")
            };
        }

        async Task IPipeMessageSerializer.WriteMessageAsync<T>(Stream stream, T message, CancellationToken cancellationToken = default)
            where T : class
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(message, jsonOptions);
            if (json.Length > ChatMessageConstants.MaximumMessageSize)
            {
                throw new ArgumentException("Message too large", nameof(message));
            }

            // Определяем тип для заголовка
            var typeTag = message switch
            {
                ChatMessage => MessageTypeTag.ChatMessage,
                RoomInfoMessage => MessageTypeTag.RoomInfo,
                DiscoveredRoom => MessageTypeTag.DiscoveredRoom,
                _ => throw new ArgumentException($"Unsupported message type: {typeof(T).Name}")
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

        private enum MessageTypeTag : byte
        {
            ChatMessage = 0,
            RoomInfo = 1,
            DiscoveredRoom = 2,
        }
    }
}
