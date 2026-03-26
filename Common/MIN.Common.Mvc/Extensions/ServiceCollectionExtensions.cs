using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

    /// <summary>
    /// Регистрирует списком реализации с удалением уже зарегистрированных
    /// </summary>
    /// <typeparam name="TService">Тип, для которого осуществляется регистрация</typeparam>
    /// <typeparam name="TAnchor">Якорь для определения сборки</typeparam>
    /// <param name="services"><inheritdoc cref="IServiceCollection"/></param>
    /// <param name="lifetime"><inheritdoc cref="ServiceLifetime"/></param>
    public static void RegisterMultipleInterfacesAssignableTo<TService, TAnchor>(this IServiceCollection services, ServiceLifetime lifetime)
    {
        var serviceType = typeof(TService);
        var types = typeof(TAnchor).Assembly.GetTypes()
            .Where(t => serviceType.IsAssignableFrom(t) &&
                        !(t.IsAbstract ||
                          t.IsInterface));

        foreach (var type in types)
        {
            services.TryAddEnumerable(new ServiceDescriptor(serviceType, type, lifetime));
        }
    }

    private static Func<byte[], IMessage> CreateDeserializer(Type messageType)
    {
        var options = JsonMessageSerializer.GetSerializerOptions();
        return data => (IMessage)JsonSerializer.Deserialize(data, messageType, options)!;
    }
}
