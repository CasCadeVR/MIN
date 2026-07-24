using MIN.Desktop.Contracts.Interfaces;

namespace MIN.Desktop.ViewModels.Base.Interfaces;

/// <summary>
/// View модель
/// </summary>
public interface IViewModel : IReferenceCommandReceiver
{
    /// <summary>
    /// Освободить ресурсы
    /// </summary>
    void Dispose();
}
