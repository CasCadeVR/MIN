using Microsoft.Extensions.DependencyInjection;
using MIN.Common.Core.Contracts.Interfaces;
using MIN.Common.Mvc;
using MIN.Common.Mvc.Extensions;
using MIN.Core.Handlers.Contracts;
using MIN.Core.Messaging.Contracts.Interfaces;
using MIN.FileTransfer.DI.FeatureCollection;
using MIN.FileTransfer.Handlers;
using MIN.FileTransfer.Messaging;
using MIN.FileTransfer.Services;

namespace MIN.FileTransfer.DI;

/// <summary>
/// Модуль регистрации зависимостей для FileTransfer
/// </summary>
public class FileTransferModule : Module
{
    /// <inheritdoc />
    protected override void Load(IServiceCollection services)
    {
        services.RegisterMultipleInterfacesAssignableFromAnchor<IMessage, IFileTransferMessagingAnchor>(ServiceLifetime.Singleton);
        services.RegisterMultipleInterfacesAssignableFromAnchor<IMessageHandler, IFileTransferHandlerAnchor>(ServiceLifetime.Singleton);
        services.RegisterMultipleInterfacesAssignableTo<IHostedService, FileMonitorService>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<FileTransferService>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<FileHelperService>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<FileStorageService>(ServiceLifetime.Singleton);
        services.RegisterAsImplementedInterfaces<FileTransferFeatureCollection>(ServiceLifetime.Singleton);
    }
}
