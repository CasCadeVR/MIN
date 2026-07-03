using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.Contracts.Interfaces;

namespace MIN.Desktop.Infrastructure.Extensions;

/// <summary>
/// Расширения для <see cref="IReferenceCommandReceiver"/>
/// </summary>
public static class ReferenceCommandReceiverExtensions
{
    /// <summary>
    /// Зарегистрировать обработчик комманд
    /// </summary>
    public static void RegisterMessageListener<T, TReceiver>(this TReceiver receiver, Func<T, TReceiver, Task> asyncFunc) where T : class where TReceiver : IReferenceCommandReceiver
    {
        if (WeakReferenceMessenger.Default.IsRegistered<T>(receiver))
        {
            WeakReferenceMessenger.Default.Unregister<T>(receiver);
        }
        WeakReferenceMessenger.Default.Register<T>(receiver, (_, message) => asyncFunc(message, receiver));
    }

    /// <summary>
    /// Зарегистрировать обработчик комманд
    /// </summary>
    public static void RegisterMessageListener<T, TReceiver>(this TReceiver receiver, Action<T, TReceiver> action) where T : class where TReceiver : IReferenceCommandReceiver
    {
        if (WeakReferenceMessenger.Default.IsRegistered<T>(receiver))
        {
            WeakReferenceMessenger.Default.Unregister<T>(receiver);
        }
        WeakReferenceMessenger.Default.Register<T>(receiver, (_, message) => action(message, receiver));
    }
}
