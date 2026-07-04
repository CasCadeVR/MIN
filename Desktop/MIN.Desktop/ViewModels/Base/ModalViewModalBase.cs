using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Interfaces;
using MIN.Desktop.Infrastructure.Extensions;

namespace MIN.Desktop.ViewModels.Base;

/// <summary>
/// Базовая view модель для модальных окон
/// </summary>
public abstract partial class ModalViewModelBase : ObservableValidator, IReferenceCommandReceiver
{
    [ObservableProperty]
    public partial ButtonOptions? SelectedOption { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ModalViewModelBase"/>
    /// </summary>
    protected ModalViewModelBase()
    {
        // Всегда сначала запускайте проверку, чтобы установить значение параметра HasErrors (т.е. активировать логику CanExecute).
        ValidateAllProperties();
    }

    /// <summary>
    /// Переопределение преобразования в тип bool
    /// </summary>
    public static implicit operator bool(ModalViewModelBase self)
    {
        return self is { HasErrors: false } and not { SelectedOption: null or ButtonOptions.No };
    }

    /// <summary>
    /// Закрывает диалоговое окно. По умолчанию результат диалога — «cancelled»
    /// </summary>
    /// <param name="buttonOptions">Результат диалога, который необходимо установить перед закрытием</param>
    [RelayCommand]
    public void Close(ButtonOptions? buttonOptions = null)
    {
        if (buttonOptions != null)
        {
            SelectedOption = buttonOptions;
        }
        ((IClassicDesktopStyleApplicationLifetime)Application.Current?.ApplicationLifetime!).Windows.FirstOrDefault(w => w.DataContext == this)?.CloseByUser(this);
    }

    /// <summary>
    /// Освободить ресурсы
    /// </summary>
    public virtual void Dispose() => WeakReferenceMessenger.Default.UnregisterAll(this);
}
