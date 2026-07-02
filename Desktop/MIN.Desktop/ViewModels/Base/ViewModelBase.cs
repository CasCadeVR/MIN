using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.Infrastructure.Interfaces;

namespace MIN.Desktop.ViewModels.Base;

/// <summary>
/// Базовая view модель
/// </summary>
public abstract class ViewModelBase : ObservableObject, IReferenceCommandReceiver
{
    /// <summary>
    /// Освободить ресурсы
    /// </summary>
    public virtual void Dispose() => WeakReferenceMessenger.Default.UnregisterAll(this);
}
