using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Core.Transport.NamedPipes.Models;

namespace MIN.Core.Serialization.Json.Services
{
    /// <summary>
    /// <see cref="JsonConverter"/> для <see cref="IEndpoint"/>
    /// </summary>
    public class IEndpointConverter : JsonConverter<IEndpoint>
    {
        /// <summary>
        /// <inheritdoc cref="JsonConverter{T}.Read(ref Utf8JsonReader, Type, JsonSerializerOptions)"/>
        /// </summary>
        public override IEndpoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (root.TryGetProperty("pipeName", out _))
            {
                return JsonSerializer.Deserialize<NamedPipeEndpoint>(root.GetRawText(), options)
                    ?? throw new InvalidCastException($"Не удалось распарсить Endpoint");
            }

            throw new NotSupportedException("Неизвестный тип Endpoint");
        }

        /// <summary>
        /// <inheritdoc cref="JsonConverter{T}.Write(Utf8JsonWriter, T, JsonSerializerOptions)"/>
        /// </summary>
        public override void Write(Utf8JsonWriter writer, IEndpoint value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
