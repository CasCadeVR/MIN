using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.Core.Serialization.Contracts;
using MIN.Core.Serialization.Json;

namespace MIN.Common.Mvc.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует все типы сообщений из указанной сборки в реестре десериализаторов
    /// </summary>
    /// <typeparam name="TMarker">Тип-маркер из сборки, содержащей сообщения</typeparam>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns>Коллекция сервисов для цепочки вызовов</returns>
    public static IServiceCollection RegisterMessagesFromAnchor<TMarker>(this IServiceCollection services)
    {
        var assembly = typeof(TMarker).Assembly;
        var messageTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IMessage).IsAssignableFrom(t))
            .ToList();

        if (messageTypes.Count == 0)
        {
            return services;
        }

        services.AddSingleton(sp =>
        {
            var registry = sp.GetRequiredService<IDeserializerRegistry>();

            foreach (var type in messageTypes)
            {
                var instance = (IMessage)Activator.CreateInstance(type)!;
                var tag = instance.TypeTag;
                var deserializer = CreateDeserializer(type);
                registry.RegisterDeserializer(tag, deserializer);
            }

            return registry;
        });

        return services;
    }

    /// <summary>
    /// Регистрирует все интерфейсы указанного типа
    /// </summary>
    /// <param name="services"><inheritdoc cref="IServiceCollection"/></param>
    /// <param name="lifetime"><inheritdoc cref="ServiceLifetime"/></param>
    /// <typeparam name="TService">Тип, для которого осуществляется регистрация</typeparam>
    public static void RegisterAsImplementedInterfaces<TService>(this IServiceCollection services, ServiceLifetime lifetime)
    {
        services.TryAdd(new ServiceDescriptor(typeof(TService), typeof(TService), lifetime));
        var interfaces = typeof(TService).GetTypeInfo()
            .ImplementedInterfaces
            .Where(i => i != typeof(IDisposable) && (i.IsPublic));

        foreach (Type interfaceType in interfaces)
        {
            services.TryAdd(new ServiceDescriptor(interfaceType,
                provider => provider.GetRequiredService(typeof(TService)),
                lifetime));
        }
    }

    /// <summary>
    /// Регистрирует все интерфейсы инстансов в указанной сборке для указанного маркерного интерфейса
    /// </summary>
    /// <param name="services"><inheritdoc cref="IServiceCollection"/></param>
    /// <param name="lifetime"><inheritdoc cref="ServiceLifetime"/></param>
    /// <typeparam name="TInterface">Тип, для которого осуществляется регистрация</typeparam>
    public static void RegisterAssemblyInterfacesAssignableTo<TInterface>(this IServiceCollection services, ServiceLifetime lifetime)
    {
        var serviceType = typeof(TInterface);
        var types = serviceType.Assembly.GetTypes()
            .Where(p => serviceType.IsAssignableFrom(p) &&
                        !(p.IsAbstract ||
                          p.IsInterface));
        foreach (var type in types)
        {
            services.TryAdd(new ServiceDescriptor(type, type, lifetime));
            var interfaces = type.GetTypeInfo()
                .ImplementedInterfaces
                .Where(i => i != typeof(IDisposable) &&
                            i.IsPublic &&
                            i != serviceType);

            foreach (Type interfaceType in interfaces)
            {
                services.TryAdd(new ServiceDescriptor(interfaceType,
                    provider => provider.GetRequiredService(type),
                    lifetime));
            }
        }
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

    /// <summary>
    /// Регистрация модуля в <see cref="IServiceCollection"/>
    /// </summary>
    public static void RegisterModule<TModule>(this IServiceCollection services)
    {
        if (Activator.CreateInstance(typeof(TModule)) is Module module)
        {
            module.Configure(services);
        }
    }

    private static Func<byte[], IMessage> CreateDeserializer(Type messageType)
    {
        var options = JsonMessageSerializer.GetSerializerOptions();
        return data => (IMessage)JsonSerializer.Deserialize(data, messageType, options)!;
    }
}
