using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Core.Serialization.Contracts;

namespace MIN.Core.Serialization.Json.Services
{
    /// <summary>
    /// Предоставляет настройки JSON-сериализации для всего приложения
    /// </summary>
    public static class JsonOptionsProvider
    {
        private static JsonSerializerOptions options = default!;

        /// <summary>
        /// Инициализировать настройки JSON-сериализации
        /// </summary>
        public static void Initialize(IMessageSerializer messageSerializer)
        {
            options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new IEndpointConverter(),
                    new IMessageConverter(messageSerializer),
                }
            };
        }

        /// <summary>
        /// Возвращает экземпляр настроек JSON-сериализации
        /// </summary>
        public static JsonSerializerOptions Options => options;
    }
}
