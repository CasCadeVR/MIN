using System;
using CommunityToolkit.Mvvm.ComponentModel;
using MIN.Core.Transport.TcpSockets.Models;
using MIN.Desktop.ViewModels.Base;

namespace MIN.Desktop.ViewModels.Modals;

/// <summary>
/// Модель окна логов
/// </summary>
public partial class LogViewModel : ModalViewModelBase
{
    [ObservableProperty]
    public partial string IpAddress { get; set; } = "";

    /// <summary>
    /// Полученная конечная точка
    /// </summary>
    public TcpEndpoint Endpoint { get; set; } = new();

    /// <summary>
    /// Событие по нажатию на кнопку
    /// </summary>
    public Action? OnConnect { get; set; }
}
