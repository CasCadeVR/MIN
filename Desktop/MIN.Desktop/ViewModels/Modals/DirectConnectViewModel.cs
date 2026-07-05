using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIN.Core.Transport.TcpSockets.Models;
using MIN.Desktop.Infrastructure.Services;
using MIN.Desktop.Infrastructure.Validators;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Modals;

/// <summary>
/// Модель окна подключения напрямую
/// </summary>
public partial class DirectConnectViewModel : ModalViewModelBase
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    [Required]
    [IpAddressText]
    public partial string IpAddress { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    [Required]
    [Range(0, 65536)]
    [NotifyDataErrorInfo]
    public partial int Port { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    public partial bool IsConnecting { get; set; }

    /// <summary>
    /// Полученная конечная точка
    /// </summary>
    public TcpEndpoint Endpoint { get; set; } = new();

    /// <summary>
    /// Событие по нажатию на кнопку
    /// </summary>
    public Action? OnConnect { get; set; }

    /// <summary>
    /// Включить кнопку подключения обратно
    /// </summary>
    public void EnableConnectButton()
    {
        IsConnecting = false;
    }

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private void Connect()
    {
        Endpoint.IPAddress = IpAddress;
        Endpoint.Port = Convert.ToInt32(Port);

        IsConnecting = true;
        OnConnect?.Invoke();
    }

    private void TryParsePort()
    {
        if (IpAddressParser.TryParseIpAddress(IpAddress, out var gottenIpAddress, out var port))
        {
            Port = port;
            IpAddress = gottenIpAddress;
        }
    }

    private bool CanConnect() => !HasErrors && !IsConnecting;
}
