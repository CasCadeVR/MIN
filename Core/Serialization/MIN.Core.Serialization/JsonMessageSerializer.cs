using System.Collections.Concurrent;
using System.Text.Json;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;

namespace MIN.Core.Serialization.Json;

/// <summary>
/// Реализация сериализатора на основе Json
/// </summary>
public sealed class JsonMessageSerializer : IMessageSerializer
{
    private readonly IEnumerable<IMessage> messageTypes;
    private readonly ConcurrentDictionary<MessageTypeTag, Func<byte[], IMessage>> deserializers = new();

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="JsonMessageSerializer"/>
    /// </summary>
    public JsonMessageSerializer(IEnumerable<IMessage> messageTypes)
    {
        this.messageTypes = messageTypes;
        InitializeDeserializers();
    }

    private void InitializeDeserializers()
    {
        foreach (var type in messageTypes)
        {
            var messageType = type.GetType();
            var instance = (IMessage)Activator.CreateInstance(messageType)!;
            var tag = instance.TypeTag;
            var deserializer = CreateDeserializer(messageType);
            if (!deserializers.TryAdd(tag, deserializer))
            {
                throw new InvalidOperationException($"Deserializer for tag {tag} already registered");
            }
        }
    }

    /// <summary>
    /// Настройки сериализации
    /// </summary>
    public JsonSerializerOptions SerializerOptions = null!;

    byte[] IMessageSerializer.Serialize(IMessage message)
        => JsonSerializer.SerializeToUtf8Bytes(message, message.GetType(), SerializerOptions);

    IMessage IMessageSerializer.Deserialize(byte[] data)
    {
        using var doc = JsonDocument.Parse(data);
        var root = doc.RootElement;

        if (!root.TryGetProperty("typeTag", out var typeTagElement) && !root.TryGetProperty("TypeTag", out typeTagElement))
        {
            throw new InvalidOperationException("Missing TypeTag property in message JSON");
        }

        var typeTag = (MessageTypeTag)typeTagElement.GetByte();
        deserializers.TryGetValue(typeTag, out var deserializer);

        return deserializer == null
            ? throw new NotSupportedException($"No deserializer registered for message type {typeTag}")
            : deserializer(data);
    }

    private Func<byte[], IMessage> CreateDeserializer(Type messageType)
        => data => (IMessage)JsonSerializer.Deserialize(data, messageType, SerializerOptions)!;
}
