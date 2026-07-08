using System;
using System.Reflection;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MIN.Common.Mvc.Extensions;
using MIN.Desktop.Contracts.Attributes;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Contracts.Models;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base.Interfaces;
using MIN.Desktop.ViewModels.Pages;
using MIN.Desktop.ViewModels.Windows;
using MIN.Desktop.Views;
using MIN.Desktop.Views.Base;
using MIN.DI;
using ServiceScan.SourceGenerator;

namespace MIN.Desktop.Infrastructure.Extensions;

/// <summary>
/// Расширения для <see cref="IServiceCollection"/> для приложения
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавить все сервисы для приложения
    /// </summary>
    public static IServiceCollection AddAppServices(this IServiceCollection services)
        => services
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
            })
            .AddDialogs()
            .AddViews()
            .AddViewModels()
            .AddTransient<ChatViewModel>() // ← перезаписывает Singleton на Transient
            .AddTransient<ChatSideBarViewModel>(); // ← перезаписывает Singleton на Transient

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.RegisterAsImplementedInterfaces<CtsProvider>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<SettingsProvider>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<NotificationService>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<DialogService>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<ChatViewModelFactory>(ServiceLifetime.Singleton);
        services.RegisterModule<MinModule>();
        return services;
    }

    [ScanForTypes(AttributeFilter = typeof(ModalForViewModelAttribute), Handler = nameof(AddDialog))]
    private static partial IServiceCollection AddDialogs(this IServiceCollection services);

    [GenerateServiceRegistrations(AssignableTo = typeof(RoutableViewBase<>), ExcludeAssignableTo = typeof(MainWindow), AsSelf = true)]
    private static partial IServiceCollection AddViews(this IServiceCollection services);

    [GenerateServiceRegistrations(AssignableTo = typeof(IViewModel), ExcludeAssignableTo = typeof(MainWindowViewModel), AsSelf = true, Lifetime = ServiceLifetime.Singleton)]
    private static partial IServiceCollection AddViewModels(this IServiceCollection services);

    private static void AddDialog<TDialog>(this IServiceCollection services) where TDialog : ModalViewBase
    {
        services.AddTransient<TDialog>();
        services.TryAddTransient(GetViewModelType());
        services.AddSingleton(provider => new ModalMapping(GetViewModelType(), viewModel =>
        {
            TDialog dialog = provider.GetRequiredService<TDialog>();
            dialog.DataContext = provider.GetRequiredService(viewModel);
            return dialog;
        }));
        static Type GetViewModelType() => typeof(TDialog).GetCustomAttribute<ModalForViewModelAttribute>()?.ViewModelType
            ?? throw new Exception($"No ViewModel assigned to {typeof(TDialog).Name}");
    }
}
