using System;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Transport.Contracts.Interfaces;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Cards;

/// <summary>
/// View модель строчки адреса соединения
/// </summary>
public partial class ConnectionAddressViewModel : CardViewModelBase, IDisposable
{
    private readonly IClipboard? clipboard;

    /// <summary>
    /// Место получения адреса
    /// </summary>
    public string Origin { get; }

    /// <summary>
    /// Адрес
    /// </summary>
    public string Address { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ConnectionAddressViewModel"/>
    /// </summary>
    public ConnectionAddressViewModel(IEndpoint address, IClipboard? clipboard)
    {
        Origin = address.GetOrigin();
        Address = address.GetAddress();
        this.clipboard = clipboard;
    }

    [RelayCommand]
    private async Task CopyToClipBoard(string message)
    {
        if (clipboard != null)
        {
            await clipboard.SetTextAsync(message);
            InAppNotifier.Info("Скопировано в буфер обмена");
        }
    }
}
