using System.Text.Json;
using Microsoft.Extensions.Hosting;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;

namespace MIN.Core.Serialization.Json.Services;

/// <summary>
/// Сервис инициализации десериализаторов
/// </summary>
public sealed class JsonDeserializerInitializer : IHostedService
{
    private readonly IMessageSerializer serializer;
    private readonly IDeserializerRegistry registry;
    private readonly IEnumerable<IMessage> messageTypes;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="JsonDeserializerInitializer"/>
    /// </summary>
    public JsonDeserializerInitializer(IMessageSerializer serializer, IDeserializerRegistry registry, IEnumerable<IMessage> messageTypes)
    {
        this.serializer = serializer;
        this.registry = registry;
        this.messageTypes = messageTypes;
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        JsonOptionsProvider.Initialize(serializer);

        foreach (var type in messageTypes)
        {
            var messageType = type.GetType();
            var instance = (IMessage)Activator.CreateInstance(messageType)!;
            var tag = instance.TypeTag;
            var deserializer = CreateDeserializer(messageType);
            registry.RegisterDeserializer(tag, deserializer);
        }

       if (serializer is JsonMessageSerializer jsonSerializer)
        {
            jsonSerializer.options = JsonOptionsProvider.Options;
        }

        return Task.CompletedTask;
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static Func<byte[], IMessage> CreateDeserializer(Type messageType)
        => data => (IMessage)JsonSerializer.Deserialize(data, messageType, JsonOptionsProvider.Options)!;
}
