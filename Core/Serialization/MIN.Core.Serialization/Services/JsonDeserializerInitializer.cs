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
    private readonly IDeserializerRegistry registry;
    private readonly IEnumerable<Type> messageTypes;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="JsonDeserializerInitializer"/>
    /// </summary>
    public JsonDeserializerInitializer(IDeserializerRegistry registry, IEnumerable<Type> messageTypes)
    {
        this.registry = registry;
        this.messageTypes = messageTypes;
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        foreach (var type in messageTypes)
        {
            var instance = (IMessage)Activator.CreateInstance(type)!;
            var tag = instance.TypeTag;
            var deserializer = CreateDeserializer(type);
            registry.RegisterDeserializer(tag, deserializer);
        }

        return Task.CompletedTask;
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static Func<byte[], IMessage> CreateDeserializer(Type messageType)
    {
        var options = JsonMessageSerializer.GetSerializerOptions();
        return data => (IMessage)JsonSerializer.Deserialize(data, messageType, options)!;
    }
}
