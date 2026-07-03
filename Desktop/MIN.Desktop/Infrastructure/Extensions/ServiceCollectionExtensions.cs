using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Mvc.Extensions;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;
using MIN.Desktop.ViewModels.Windows;
using MIN.Desktop.Views;
using MIN.Desktop.Views.Base;
using MIN.DI;

namespace MIN.Desktop.Infrastructure.Extensions;

/// <summary>
/// Расширения для <see cref="IServiceCollection"/> для приложения
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавить все сервисы для приложения
    /// </summary>
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        var resultServices = services
            // Domain APIs
            //.AddSingleton(_ => KeyValueStore.Instance)
            // Services
            //.AddSingleton<DialogService>()
            // UI
            .AddServices()
            .AddSingleton<Window, MainWindow>()
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<Func<IMultiRoutingWindow>>(provider => provider.GetRequiredService<MainWindowViewModel>)
            .AddSingleton<Func<Window>>(provider => () =>
            {
                Window window = provider.GetRequiredService<Window>();
                window.DataContext = provider.GetRequiredService<MainWindowViewModel>();
                return window;
            });
        //.AddDialogs()
        resultServices.RegisterAssemblyInterfacesAssignableTo(typeof(RoutableViewBase<>), ServiceLifetime.Singleton, typeof(MainWindow));
        resultServices.RegisterAssemblyInterfacesAssignableTo<ViewModelBase>(ServiceLifetime.Singleton, typeof(MainWindowViewModel));
        return resultServices;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.RegisterAsImplementedInterfaces<CtsProvider>(ServiceLifetime.Singleton);
        services.RegisterModule<MinModule>();
        return services;
    }

    //[GenerateServiceRegistrations(AttributeFilter = typeof(ModalForViewModelAttribute), CustomHandler = nameof(AddDialog))]
    //private static partial IServiceCollection AddDialogs(this IServiceCollection services);

    //[GenerateServiceRegistrations(AssignableTo = typeof(RoutableViewBase<>), ExcludeAssignableTo = typeof(MainWindow), AsSelf = true)]
    //private static IServiceCollection AddViews(this IServiceCollection services);

    //[GenerateServiceRegistrations(AssignableTo = typeof(ViewModelBase), ExcludeAssignableTo = typeof(MainWindowViewModel), AsSelf = true, Lifetime = ServiceLifetime.Singleton)]
    //private static IServiceCollection AddViewModels(this IServiceCollection services);

    //private static void AddDialog<TDialog>(this IServiceCollection services) where TDialog : ModalBase
    //{
    //    services.AddTransient<TDialog>();
    //    services.TryAddTransient(GetViewModelType());
    //    services.AddSingleton(provider => new DialogService.Mapping(GetViewModelType(), viewModel =>
    //    {
    //        TDialog dialog = provider.GetRequiredService<TDialog>();
    //        dialog.DataContext = provider.GetRequiredService(viewModel);
    //        return dialog;
    //    }));
    //    static Type GetViewModelType() => typeof(TDialog).GetCustomAttribute<ModalForViewModelAttribute>()?.ViewModelType ?? throw new Exception($"No ViewModel assigned to {typeof(TDialog).Name}");
    //}
}
