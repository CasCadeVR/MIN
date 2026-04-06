using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;

namespace MIN.Core.Serialization.Json.Services
{
    public class IMessageConverter : JsonConverter<IMessage>
    {
        private readonly IMessageSerializer serializer;

        public IMessageConverter(IMessageSerializer serializer)
        {
            this.serializer = serializer;
        }

        public override IMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            var bytes = Encoding.UTF8.GetBytes(root.GetRawText());
            return serializer.Deserialize(bytes);
        }

        public override void Write(Utf8JsonWriter writer, IMessage value, JsonSerializerOptions options)
        {
            var bytes = serializer.Serialize(value);
            var json = Encoding.UTF8.GetString(bytes);
            writer.WriteRawValue(json);
        }
    }
}
