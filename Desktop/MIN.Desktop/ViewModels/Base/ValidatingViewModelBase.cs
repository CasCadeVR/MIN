using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.Contracts.Interfaces;

namespace MIN.Desktop.ViewModels.Base;

/// <summary>
/// Базовая view модель для валидирующих view model
/// </summary>
public abstract partial class ValidatingViewModelBase : ObservableValidator, IReferenceCommandReceiver
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ValidatingViewModelBase"/>
    /// </summary>
    protected ValidatingViewModelBase()
    {
        // Всегда сначала запускайте проверку, чтобы установить значение параметра HasErrors (т.е. активировать логику CanExecute).
        ValidateAllProperties();
    }

    /// <summary>
    /// Освободить ресурсы
    /// </summary>
    public virtual void Dispose() => WeakReferenceMessenger.Default.UnregisterAll(this);
}
