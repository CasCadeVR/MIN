using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Models.ReferenceCommands;

namespace MIN.Desktop.ViewModels.Base;

/// <summary>
/// Базовая view модель для страниц
/// </summary>
public abstract class RoutableViewModelBase : ViewModelBase
{
    /// <inheritdoc cref="ViewLayoutType"/>
    public abstract ViewLayoutType LayoutType { get; }

    /// <summary>
    /// Updates the current view container to show a different view, as is known by the TViewModel type.
    /// </summary>
    protected void ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : RoutableViewModelBase
    {
        WeakReferenceMessenger.Default.Send(new ShowViewReferenceCommand
        {
            ViewModel = viewModel
        });
    }

    /// <summary>
    /// Перейти назад
    /// </summary>
    protected void ChangeViewToPrevious()
    {
        WeakReferenceMessenger.Default.Send(new ShowPreviousViewReferenceCommand()
        {
            LayoutType = LayoutType
        });
    }

    /// <summary>
    /// Перейти назад
    /// </summary>
    /// <typeparam name="T">Тип view model, на который нужно переместиться назад</typeparam>
    protected void ChangeViewToPrevious<T>() where T : RoutableViewModelBase
    {
        WeakReferenceMessenger.Default.Send(new ShowPreviousViewReferenceCommand()
        {
            RoutableViewModelType = typeof(T),
            LayoutType = LayoutType
        });
    }
}
