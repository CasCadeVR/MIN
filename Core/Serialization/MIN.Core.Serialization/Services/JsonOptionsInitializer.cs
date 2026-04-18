using System.Text.Json;
using System.Text.Json.Serialization;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Serialization.Json.Converters;

namespace MIN.Core.Serialization.Json.Services;

/// <summary>
/// Сервис инициализации десериализаторов
/// </summary>
public sealed class JsonOptionsInitializer : IHostedService
{
    private readonly IMessageSerializer serializer;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="JsonOptionsInitializer"/>
    /// </summary>
    public JsonOptionsInitializer(IMessageSerializer serializer)
    {
        this.serializer = serializer;
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        if (serializer is JsonMessageSerializer jsonSerializer)
        {
            jsonSerializer.SerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new IEndpointConverter(),
                    new IMessageConverter(serializer),
                }
            };
        }

        return Task.CompletedTask;
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
