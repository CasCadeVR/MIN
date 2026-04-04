using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;

namespace MIN.Core.Serialization.Json.Services
{
    public class IMessageListConverter : JsonConverter<List<IMessage>>
    {
        private readonly IMessageSerializer serializer;

        public IMessageListConverter(IMessageSerializer serializer)
        {
            this.serializer = serializer;
        }

        public override List<IMessage> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var list = new List<IMessage>();
            using var doc = JsonDocument.ParseValue(ref reader);
            foreach (var element in doc.RootElement.EnumerateArray())
            {
                var rawBytes = Encoding.UTF8.GetBytes(element.GetRawText());
                list.Add(serializer.Deserialize(rawBytes));
            }
            return list;
        }

        public override void Write(Utf8JsonWriter writer, List<IMessage> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var message in value)
            {
                var bytes = serializer.Serialize(message);
                var json = Encoding.UTF8.GetString(bytes);
                writer.WriteRawValue(json);
            }
            writer.WriteEndArray();
        }


        //public override List<IMessage> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //{
        //    var list = new List<IMessage>();
        //    using var doc = JsonDocument.ParseValue(ref reader);
        //    foreach (var element in doc.RootElement.EnumerateArray())
        //    {
        //        var typeTag = (MessageTypeTag)element.GetProperty("typeTag").GetByte();
        //        IMessage message;

        //        switch (typeTag)
        //        {
        //            case MessageTypeTag.SystemMessage:
        //                message = JsonSerializer.Deserialize<SystemTextMessage>(element.GetRawText(), options);
        //                break;

        //            case MessageTypeTag.ChatTextMessage:
        //                message = JsonSerializer.Deserialize<ChatTextMessage>(element.GetRawText(), options);
        //                break;

        //            default:
        //                throw new NotSupportedException($"Unknown message type {typeTag}");
        //        }

        //        list.Add(message);
        //    }
        //    return list;
        //}

        //public override void Write(Utf8JsonWriter writer, List<IMessage> value, JsonSerializerOptions options)
        //{
        //    writer.WriteStartArray();
        //    foreach (var message in value)
        //    {
        //        JsonSerializer.Serialize(writer, message, message.GetType(), options);
        //    }
        //    writer.WriteEndArray();
        //}
    }
}
