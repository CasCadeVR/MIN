using System.Collections.Concurrent;
using System.Diagnostics;
using MIN.Core.Messaging.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;

namespace MIN.Core.Serialization.Json
{
    /// <inheritdoc cref="IDeserializerRegistry"/>
    public sealed class JsonDeserializerRegistry : IDeserializerRegistry
    {
        private readonly ConcurrentDictionary<MessageTypeTag, Func<byte[], IMessage>> deserializers = new();

        public JsonDeserializerRegistry()
        {
            Debug.WriteLine("JsonDeserializerRegistry constructor START");
        }

        void IDeserializerRegistry.RegisterDeserializer(MessageTypeTag tag, Func<byte[], IMessage> deserializer)
        {
            if (!deserializers.TryAdd(tag, deserializer))
            {
                throw new InvalidOperationException($"Deserializer for tag {tag} already registered");
            }
        }

        Func<byte[], IMessage>? IDeserializerRegistry.GetDeserializer(MessageTypeTag tag)
            => deserializers.TryGetValue(tag, out var deserializer) ? deserializer : null;
    }
}
