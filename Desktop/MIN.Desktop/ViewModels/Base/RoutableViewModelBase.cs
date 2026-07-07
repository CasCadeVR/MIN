using CommunityToolkit.Mvvm.Messaging;
using MIN.Desktop.Contracts.Enums;
using MIN.Desktop.Contracts.Models.ReferenceCommands;
using MIN.Desktop.ViewModels.Base.Interfaces;

namespace MIN.Desktop.ViewModels.Base;

/// <summary>
/// Базовая view модель для страниц
/// </summary>
public abstract class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
{
    /// <inheritdoc cref="ViewLayoutType"/>
    public abstract ViewLayoutType LayoutType { get; }

    /// <summary>
    /// Свящана ли эта панель с центральной, и должна ли она убраться во время перехода с неё
    /// </summary>
    public virtual bool RelatedToCentral { get; }

    /// <inheritdoc />
    public void ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel
    {
        WeakReferenceMessenger.Default.Send(new ShowViewReferenceCommand(viewModel));
    }

    /// <inheritdoc />
    public void CloseView()
    {
        WeakReferenceMessenger.Default.Send(new CloseViewReferenceCommand(LayoutType));
    }

    /// <inheritdoc />
    public void ChangeViewToPrevious()
    {
        WeakReferenceMessenger.Default.Send(new ShowPreviousViewReferenceCommand(LayoutType, null));
    }

    /// <inheritdoc />
    public void ChangeViewToPrevious<T>() where T : IRoutableViewModel
    {
        WeakReferenceMessenger.Default.Send(new ShowPreviousViewReferenceCommand(LayoutType, typeof(T)));
    }
}
