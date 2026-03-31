using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;

namespace MIN.Core.Serialization.Json;

/// <summary>
/// Реализация сериализатора на основе Json
/// </summary>
public sealed class JsonMessageSerializer : IMessageSerializer
{
    private static readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    private readonly IDeserializerRegistry deserializerRegistry;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="JsonMessageSerializer"/>
    /// </summary>
    public JsonMessageSerializer(IDeserializerRegistry deserializerRegistry)
    {
        this.deserializerRegistry = deserializerRegistry;
    }

    /// <summary>
    /// Получить <see cref="JsonSerializerOptions"/>
    /// </summary>
    public static JsonSerializerOptions GetSerializerOptions() => serializerOptions;

    byte[] IMessageSerializer.Serialize(IMessage message)
    {
        return JsonSerializer.SerializeToUtf8Bytes(message, message.GetType(), serializerOptions);
    }

    IMessage IMessageSerializer.Deserialize(byte[] data)
    {
        using var doc = JsonDocument.Parse(data);
        var root = doc.RootElement;

        if (!root.TryGetProperty("typeTag", out var typeTagElement))
        {
            throw new InvalidOperationException("Missing TypeTag property in message JSON");
        }

        var typeTag = (MessageTypeTag)typeTagElement.GetByte();

        var deserializer = deserializerRegistry.GetDeserializer(typeTag);
        if (deserializer == null)
        {
            throw new NotSupportedException($"No deserializer registered for message type {typeTag}");
        }

        return deserializer(data);
    }
}
