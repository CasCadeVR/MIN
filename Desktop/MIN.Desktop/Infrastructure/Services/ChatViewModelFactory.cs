using System;
using Microsoft.Extensions.DependencyInjection;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.ViewModels.Pages;

namespace MIN.Desktop.Infrastructure.Services;

internal class ChatViewModelFactory : IChatViewModelFactory
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ChatViewModelFactory"/>
    /// </summary>
    public ChatViewModelFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public ChatViewModel Create() => serviceProvider.GetRequiredService<ChatViewModel>();
}
