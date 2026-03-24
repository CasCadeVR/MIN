using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using MIN.Messaging.Contracts;
using MIN.Messaging.Contracts.Interfaces;
using MIN.Serialization.Contracts;
using MIN.Serialization.Json;

namespace MIN.Common.Mvc.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует JsonMessageSerializer и автоматически находит все типы сообщений в сборке маркера
    /// </summary>
    /// <typeparam name="TMarker">Тип-маркер из сборки, содержащей сообщения</typeparam>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns>Коллекция сервисов для цепочки вызовов</returns>
    public static IServiceCollection AddJsonMessageSerializer<TMarker>(this IServiceCollection services)
    {
        var assembly = typeof(TMarker).Assembly;
        var messageTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IMessage).IsAssignableFrom(t))
            .ToList();

        if (messageTypes.Count == 0)
        {
            throw new InvalidOperationException("No message types found in the specified assembly");
        }

        var deserializers = new Dictionary<MessageTypeTag, Func<byte[], IMessage>>();

        foreach (var type in messageTypes)
        {
            var instance = (IMessage)Activator.CreateInstance(type)!;
            var tag = instance.TypeTag;

            if (deserializers.ContainsKey(tag))
            {
                throw new InvalidOperationException($"Duplicate message type tag {tag} for types {type.Name}");
            }

            deserializers[tag] = CreateDeserializer(type);
        }

        services.AddSingleton<IMessageSerializer>(_ => new JsonMessageSerializer(deserializers));
        return services;
    }

    private static Func<byte[], IMessage> CreateDeserializer(Type messageType)
    {
        var options = JsonMessageSerializer.GetSerializerOptions();
        return data => (IMessage)JsonSerializer.Deserialize(data, messageType, options)!;
    }
}
