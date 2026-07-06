using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.ViewModels.Base.Interfaces;

namespace MIN.Desktop.ViewModels.Base;

/// <summary>
/// Базовая view модель
/// </summary>
public abstract class ViewModelBase : ObservableObject, IViewModel
{
    /// <summary>
    /// Освободить ресурсы
    /// </summary>
    public virtual void Dispose() => WeakReferenceMessenger.Default.UnregisterAll(this);
}
